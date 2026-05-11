using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUITestApp.ViewModels;

public class ViewModelBase(ILogger logger)
    : ObservableObject
{
    #region Properties

    protected ILogger Logger => logger;

    #endregion
}
