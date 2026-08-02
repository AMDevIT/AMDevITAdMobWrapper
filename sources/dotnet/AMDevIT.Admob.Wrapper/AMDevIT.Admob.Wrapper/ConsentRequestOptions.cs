namespace AMDevIT.Admob.Wrapper;

public sealed record ConsentRequestOptions(bool TagForUnderAgeOfConsent = false,
                                           ConsentDebugParameters? DebugParameters = null);
