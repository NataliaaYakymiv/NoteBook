
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using XLabs.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using XLabs.Platform.Services.Media;

namespace NoteBook.Droid
{
    [Activity(Label = "NoteBook", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity//XFormsApplicationDroid
    {
        protected override void OnCreate(Bundle bundle)
        {
           // TabLayoutResource = Resource.Layout.Tabbar;
           // ToolbarResource = Resource.Layout.Toolbar;

            //base.OnCreate(bundle);

            SimpleContainer container = new SimpleContainer();
            container.Register<IDevice>(t => AndroidDevice.CurrentDevice);
            container.Register<IDisplay>(t => t.Resolve<IDevice>().Display);
            container.Register<INetwork>(t => t.Resolve<IDevice>().Network);
            container.Register<IMediaPicker>(t => t.Resolve<IDevice>().MediaPicker);

            Resolver.SetResolver(container.GetResolver());


            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
            // global::Xamarin.Forms.Forms.Init(this, bundle);
            // Toolbar tb = new Toolbar(this);//FindViewById<Toolbar>(Resource.Layout.Toolbar);
            // tb.
            // this.SetActionBar(tb);

            // LoadApplication(new App());
        }
    }
}

