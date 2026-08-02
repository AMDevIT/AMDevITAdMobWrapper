namespace AMDevIT.Admob.Wrapper;

public sealed record ConsentInformationSnapshot(DateTimeOffset? LastRefresh,
                                                ConsentStatus ConsentStatus,
                                                PrivacyOptionsRequirementStatus PrivacyOptionsRequirementStatus);
