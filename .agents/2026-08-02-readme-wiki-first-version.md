# README and wiki first version

## Objective and status

Update the repository README for the current AdMob Next-Gen, UMP, logging,
MAUI, and desktop feature set, and create the first complete usage-oriented
version of the GitHub wiki. Completed.

## Decisions made

- Kept the documentation in English, consistently with the existing README
  and repository documentation.
- Added a concise feature overview and platform-support matrix to the README,
  while moving detailed setup and usage guidance into the wiki.
- Documented consent as a mandatory mobile startup gate before Mobile Ads
  initialization and ad requests.
- Documented Windows and Mac Catalyst consent as detectable, logged no-op
  implementations and banner rendering through `FallbackTemplate`; documented
  full-screen desktop services as unsupported.
- Documented Android Next-Gen `1.3.1`, the official .NET UMP binding, and the
  absence of an explicit native Gradle UMP dependency.
- Documented the iOS XCFramework as built from Google's official
  `swift-package-manager-google-mobile-ads` package `13.7.0`, with UMP supplied
  transitively.
- Corrected old README examples that advertised async methods not exposed by
  the low-level iOS binding. Native iOS ad examples now use listener callbacks;
  MAUI full-screen services remain async.
- Switched the initialized wiki submodule from detached HEAD to its local
  `master` branch before editing. No commit or push was performed.

## Affected files

- `README.md`
- `AMDevITAdMobWrapper.wiki/Home.md`
- `AMDevITAdMobWrapper.wiki/_Sidebar.md`
- `AMDevITAdMobWrapper.wiki/Getting-Started.md`
- `AMDevITAdMobWrapper.wiki/Privacy-and-Consent-UMP.md`
- `AMDevITAdMobWrapper.wiki/MAUI-Usage.md`
- `AMDevITAdMobWrapper.wiki/Android-Usage.md`
- `AMDevITAdMobWrapper.wiki/iOS-Usage.md`
- `AMDevITAdMobWrapper.wiki/Ad-Formats.md`
- `AMDevITAdMobWrapper.wiki/Logging-and-Diagnostics.md`
- `AMDevITAdMobWrapper.wiki/Desktop-Fallbacks.md`
- `AMDevITAdMobWrapper.wiki/Testing-and-Troubleshooting.md`
- `.agents/2026-08-02-readme-wiki-first-version.md`
- `.agents/context.md`

## Checks performed and results

- Cross-checked documented package and native SDK versions against project
  files: wrapper `0.1.10`, Android Next-Gen `1.3.1`, Android UMP binding
  `4.0.0.3`, and iOS Google Mobile Ads Swift package `13.7.0`.
- Cross-checked consent, logging, banner, and full-screen examples against the
  public .NET interfaces and iOS binding definitions.
- Verified that every GitHub wiki link maps to an existing page.
- Verified balanced Markdown code fences and no trailing whitespace.
- Ran Git diff whitespace checks for the tracked README and wiki home page;
  no errors were reported.
- No restore, build, or test was run because this task changes documentation
  only and does not alter source code or project configuration.

## Open issues and recommended next step

- Review the rendered GitHub wiki after publication, especially navigation and
  code-block wrapping on narrow screens.
- Commit the wiki changes inside the wiki repository first, then commit the
  updated submodule pointer together with the root README and context files in
  the parent repository. Pushes remain a manual/user-authorized operation.
