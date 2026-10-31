#!/usr/bin/env python3
"""Validate complete release packages or the three macOS preparation packages."""

from pathlib import Path
import sys
import xml.etree.ElementTree as ET
from zipfile import BadZipFile, ZipFile


def require(condition, message):
    if not condition:
        raise ValueError(message)


def verify(package_directory, version, solution_directory, native_preparation=False):
    package_ids = (
        "AMDevIT.Admob.Wrapper.Droid",
        "AMDevIT.Admob.Wrapper.iOSNative",
        "AMDevIT.Admob.Wrapper",
        "AMDevIT.Admob.Wrapper.MAUICross",
    )
    for package_id in package_ids:
        if native_preparation and package_id == "AMDevIT.Admob.Wrapper.MAUICross":
            continue
        project = ET.parse(solution_directory / package_id / f"{package_id}.csproj").getroot()
        expected_references = {
            reference.attrib["Include"]: reference.attrib["Version"]
            for reference in project.iter("PackageReference")
        }
        expected_references.update({
            Path(reference.attrib["Include"].replace("\\", "/")).stem: version
            for reference in project.iter("ProjectReference")
        })
        for extension in ("nupkg", "snupkg"):
            package_path = package_directory / f"{package_id}.{version}.{extension}"
            with ZipFile(package_path) as archive:
                require(archive.testzip() is None, f"Corrupt package: {package_path}")
                entries = archive.namelist()
                nuspecs = [name for name in entries if name.endswith(".nuspec")]
                require(len(nuspecs) == 1, f"Expected one nuspec in {package_path}")
                root = ET.fromstring(archive.read(nuspecs[0]))
                # NuGet nuspec namespaces vary with the schema revision.
                for element in root.iter():
                    element.tag = element.tag.rsplit("}", 1)[-1]
                metadata = root.find("metadata")
                require(metadata is not None, f"Missing metadata in {package_path}")
                require(metadata.findtext("id") == package_id and metadata.findtext("version") == version,
                        f"Unexpected package identity: {package_path}")
                if extension == "snupkg":
                    require(any(name.endswith(".pdb") for name in entries), f"Missing symbols: {package_path}")
                    continue
                for name in ("README.md", "admob_maui_icon_v4_wallet_block.png"):
                    require(name in entries, f"Missing {name} in {package_path}")
                license_element = metadata.find("license")
                require(license_element is not None and license_element.text == "Apache-2.0"
                        and license_element.get("type") == "expression", f"Invalid license in {package_path}")
                require(metadata.findtext("readme") == "README.md", f"Invalid README metadata in {package_path}")
                repository = metadata.find("repository")
                require(repository is not None and repository.get("commit")
                        and repository.get("url") == "https://github.com/AMDevIT/AMDevITAdMobWrapper",
                        f"Missing repository metadata in {package_path}")
                dependencies = {}
                for dependency in metadata.iter("dependency"):
                    dependencies.setdefault(dependency.get("id"), set()).add(dependency.get("version"))
                for reference_id, reference_version in expected_references.items():
                    require(reference_version in dependencies.get(reference_id, set()),
                            f"Missing dependency {reference_id} {reference_version} in {package_path}")
                require("Xamarin.GooglePlayServices.Ads" not in dependencies,
                        f"Legacy Ads SDK dependency in {package_path}")
                assemblies = [name for name in entries if name.startswith("lib/") and name.endswith(f"/{package_id}.dll")]
                require(assemblies, f"Missing library assemblies in {package_path}")
                if package_id == "AMDevIT.Admob.Wrapper.MAUICross":
                    for platform in ("android", "ios", "maccatalyst", "windows"):
                        require(any(platform in name.split("/")[1] for name in assemblies),
                                f"Missing {platform} target in {package_path}")
            print(f"Verified: {package_path.name}")


if __name__ == "__main__":
    if len(sys.argv) not in (4, 5) or (len(sys.argv) == 5 and sys.argv[4] != "--native-preparation"):
        raise SystemExit("Usage: verify-nuget.py PACKAGE_DIRECTORY VERSION SOLUTION_DIRECTORY [--native-preparation]")
    try:
        verify(Path(sys.argv[1]), sys.argv[2], Path(sys.argv[3]), native_preparation=len(sys.argv) == 5)
    except (ValueError, OSError, ET.ParseError, BadZipFile) as error:
        raise SystemExit(f"Package verification failed: {error}") from error
