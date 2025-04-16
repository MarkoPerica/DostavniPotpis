using Android.App;
using Android.Content.PM;
using Com.Datalogic.Device;
using Com.Datalogic.Decode;
using CommunityToolkit.Mvvm.Messaging;
using Android.OS;
using Android.Content;
using Android.Views.InputMethods;
using Android.Views;
using Android.Widget;
using DostavniPotpis.Messages;

namespace DostavniPotpis;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity, IReadListener
{
    BarcodeManager decoder = null;

    protected override void OnResume()
    {
        base.OnResume();

        // If the decoder instance is null, create it.
        if (decoder == null)
        {
            // Remember an onPause call will set it to null.
            decoder = new BarcodeManager();
        }

        // From here on, we want to be notified with exceptions in case of errors.
        ErrorManager.EnableExceptions(true);

        try
        {
            // add our class as a listener
            decoder.AddReadListener(this);
        }
        catch (DecodeException e)
        {
            Console.WriteLine("Error while trying to bind a listener to BarcodeManager");
        }
    }

    protected override void OnPause()
    {
        base.OnPause();

        // If we have an instance of BarcodeManager.
        if (decoder != null)
        {
            try
            {
                // Unregister our listener from it and free resources
                decoder.RemoveReadListener(this);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to remove a listener from BarcodeManager");
            }
        }
    }

    void IReadListener.OnRead(IDecodeResult decodeResult)
    {
        // Change the displayed text to the current received result.
        Console.WriteLine(decodeResult.Text);
        WeakReferenceMessenger.Default.Send(new SendBarcodeDecode(decodeResult.Text));
    }

    //Method for removing focus on entry for barcode input 
    public override bool DispatchTouchEvent(MotionEvent? e)
    {
        if (e!.Action == MotionEventActions.Down)
        {
            var focusedElement = CurrentFocus;
            if (focusedElement is EditText editText)
            {
                var editTextLocation = new int[2];
                editText.GetLocationOnScreen(editTextLocation);
                var clearTextButtonWidth = 100;
                var editTextRect = new Rect(editTextLocation[0], editTextLocation[1], editText.Width + clearTextButtonWidth, editText.Height);
                var touchPosX = (int)e.RawX;
                var touchPosY = (int)e.RawY;
                if (!editTextRect.Contains(touchPosX, touchPosY))
                {
                    editText.ClearFocus();
                    var inputService = GetSystemService(Context.InputMethodService) as InputMethodManager;
                    inputService?.HideSoftInputFromWindow(editText.WindowToken, 0);
                }
            }
        }
        return base.DispatchTouchEvent(e);
    }
}
