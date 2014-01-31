using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.GamerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace VaderpiXX.Nokia
{
    public enum FeedbackState
    {
        Inactive,
        FirstReview,
        SecondReview,
        Feedback
    }

    /// <summary>
    /// This helper class controls the behaviour of the FeedbackOverlay control
    /// When the app has been launched 5 times the initial prompt is shown
    /// If the user reviews no more prompts are shown
    /// When the app has bee launched 10 times and not been reviewed, the prompt is shown
    /// </summary>
    public class FeedbackHelper
    {
        private const string LAUNCH_COUNT = "LAUNCH_COUNT";
        private const string REVIEWED = "REVIEWED";
        private const int FIRST_COUNT = 5;
        private const int SECOND_COUNT = 10;

        private int _launchCount = 0;
        private bool _reviewed = false;

        public static readonly FeedbackHelper Default = new FeedbackHelper();

        private FeedbackState _state = FeedbackState.Inactive;

        public FeedbackState State
        {
            get { return this._state; }
            set { this._state = value; }
        }

        private string Message { get; set; }
        private string Title { get; set; }
        private string YesText { get; set; }
        private string NoText { get; set; }

        private FeedbackHelper()
        {

        }

        /// <summary>
        /// Loads last state from storage and works out the new state
        /// </summary>
        private void LoadState()
        {
            try
            {
                this._launchCount = StorageHelper.GetSetting<int>(LAUNCH_COUNT);
                this._reviewed = StorageHelper.GetSetting<bool>(REVIEWED);

                if (!this._reviewed)
                {
                    this._launchCount++;

                    if (this._launchCount == FIRST_COUNT)
                        this._state = FeedbackState.FirstReview;
                    else if (this._launchCount == SECOND_COUNT)
                        this._state = FeedbackState.SecondReview;

                    this.StoreState();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("FeedbackHelper.LoadState - Failed to load state, Exception: {0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Stores current state
        /// </summary>
        private void StoreState()
        {
            try
            {
                StorageHelper.StoreSetting(LAUNCH_COUNT, this._launchCount, true);
                StorageHelper.StoreSetting(REVIEWED, this._reviewed, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("FeedbackHelper.StoreState - Failed to store state, Exception: {0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Call when user has reviewed
        /// </summary>
        public void Reviewed()
        {
            this._reviewed = true;

            this.StoreState();
        }

        public void Initialise()
        {
            // Only load state if not trial
            if (!Microsoft.Xna.Framework.GamerServices.Guide.IsTrialMode)
            {
                this.LoadState();

                // Uncomment for testing
                // this._state = FeedbackState.FirstReview;
                // this._state = FeedbackState.SecondReview;

                if (this.State == FeedbackState.FirstReview)
                {
                    this.Title = "Enjoying VaderpiXX?";
                    this.Message = "We'd love you to rate our app 5 stars\n\nShowing us some love on the store helps us to continue to work on the app and make things even better!";
                    this.YesText = "rate 5 stars";
                    this.NoText = "no thanks";

                    this.ShowMessage();
                }
                else if (this.State == FeedbackState.SecondReview)
                {
                    this.Title = "Enjoying VaderpiXX?";
                    this.Message = "You look to be getting a lot of use out of our application!\n\nWhy not give us a 5 star rating to show your appreciation?";
                    this.YesText = "rate 5 stars";
                    this.NoText = "no thanks";

                    this.ShowMessage();
                }
            }
        }

        private void ShowMessage()
        {
            int loop = 0;

            // Check guide is not open
            while (Guide.IsVisible)
            {
                if (loop > 20) // Max 2s
                    return;
                loop++;

                System.Threading.Thread.Sleep(100);
            }

            Guide.BeginShowMessageBox(this.Title, this.Message,
                new List<string>() { this.YesText, this.NoText },
                0, MessageBoxIcon.None, (r) =>
                    {
                        var result = Guide.EndShowMessageBox(r);
                        if (result.HasValue && result.Value == 0)
                            OnYesClick();
                        else
                            OnNoClick();

                    }, null);
        }

        private void OnNoClick()
        {
            if (FeedbackHelper.Default.State == FeedbackState.FirstReview)
            {
                this.Title = "Can we make it better?";
                this.Message = "Sorry to hear you didn't want to rate VaderpiXX.\n\nTell us about your experience or suggest how we can make it even better.";
                this.YesText = "give feedback";
                this.NoText = "no thanks";

                FeedbackHelper.Default.State = FeedbackState.Feedback;
                ShowMessage();
            }
        }

        private void OnYesClick()
        {
            if (FeedbackHelper.Default.State == FeedbackState.FirstReview)
            {
                this.Review();
            }
            else if (FeedbackHelper.Default.State == FeedbackState.SecondReview)
            {
                this.Review();
            }
            else if (FeedbackHelper.Default.State == FeedbackState.Feedback)
            {
                this.Feedback();
            }
        }

        private void Review()
        {
            this.Reviewed();

            var marketplace = new MarketplaceReviewTask();
            marketplace.Show();
        }

        private void Feedback()
        {
            // Application version
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var parts = asm.FullName.Split(',');
            var version = parts[1].Split('=')[1];

            // Body text including hardware, firmware and software info
            string body = string.Format("[Your feedback here]\n\n---------------------------------\nDevice Name: {0}\nDevice Manufacturer: {1}\nDevice Firmware Version: {2}\nDevice Hardware Version: {3}\nApplication Version: {4}\n---------------------------------",
                DeviceStatus.DeviceName,
                DeviceStatus.DeviceManufacturer,
                DeviceStatus.DeviceFirmwareVersion,
                DeviceStatus.DeviceHardwareVersion,
                version);

            // Email task
            var email = new EmailComposeTask();
            email.To = "apps@bsautermeister.de";
            email.Subject = "VaderpiXX Customer Feedback";
            email.Body = body;

            email.Show();
        }
    }
}
