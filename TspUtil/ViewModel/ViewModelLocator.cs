/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:TspUtil"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using TspUtil.ViewModel;

namespace TspUtil
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MianViewModel>();
            SimpleIoc.Default.Register<SerialDev>();
            SimpleIoc.Default.Register<NetDev>();
            SimpleIoc.Default.Register<TextEitorModal>();
            SimpleIoc.Default.Register<KeySettingModal>();
        }

        public MianViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MianViewModel>();
            }
        }
        public SerialDev SerialDev
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SerialDev>();
            }
        }
        public NetDev ServDev
        {
            get
            {
                return ServiceLocator.Current.GetInstance<NetDev>();
            }
        }
        //public ClientNetDev ClientDev
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<ClientNetDev>();
        //    }
        //}
        public TextEitorModal TextModal
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TextEitorModal>();
            }
        }
        public KeySettingModal KeyBind
        {
            get
            {
                return ServiceLocator.Current.GetInstance<KeySettingModal>();
            }
        }
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}