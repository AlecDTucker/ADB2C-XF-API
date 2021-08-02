using System;
using Adb2cDemo.Droid.Services;
using Adb2cDemo.Services.Interfaces;
using Plugin.CurrentActivity;
using Xamarin.Forms;

[assembly:Dependency(typeof(ParentWindowLocatorService))]
namespace Adb2cDemo.Droid.Services
{
    public class ParentWindowLocatorService : IParentWindowLocatorService
    {
        public ParentWindowLocatorService()
        {
        }

        public object GetCurrentParentWindow()
        {
            return CrossCurrentActivity.Current.Activity;
        }
    }
}
