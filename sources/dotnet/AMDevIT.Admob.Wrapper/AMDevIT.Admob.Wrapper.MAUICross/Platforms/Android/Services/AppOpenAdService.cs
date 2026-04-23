#if ANDROID

using System;
using System.Collections.Generic;
using System.Text;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public partial class AppOpenAdService    
{
    #region Properties

    public bool IsShowing => throw new NotImplementedException();

    public bool IsLoaded => throw new NotImplementedException();

    #endregion

    #region Methods

    public Task LoadAsync(string adUnitId)
    {
        throw new NotImplementedException();
    }

    public void Show()
    {
        throw new NotImplementedException();
    }

    #endregion
}

#endif