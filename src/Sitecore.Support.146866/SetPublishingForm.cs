namespace Sitecore.Support.Shell.Applications.ContentManager.Dialogs.SetPublishing
{
    using Sitecore.Shell.Applications.ContentManager.Dialogs.SetPublishing;
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Text;
    using Sitecore.Web;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.Pages;
    using Sitecore.Web.UI.Sheer;
    using System;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public class SetPublishingForm : Sitecore.Shell.Applications.ContentManager.Dialogs.SetPublishing.SetPublishingForm
    {


        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            Item itemFromQueryString = UIUtil.GetItemFromQueryString(Context.ContentDatabase);
            Error.AssertItemFound(itemFromQueryString);
            ListString str = new ListString();
            using (new StatisticDisabler(StatisticDisablerState.ForItemsWithoutVersionOnly))
            {
                itemFromQueryString.Editing.BeginEdit();
                itemFromQueryString.Publishing.NeverPublish = !this.NeverPublish.Checked;
                itemFromQueryString.Publishing.PublishDate = DateUtil.ParseDateTime(this.Publish.Value,
                    DateTime.MinValue);
                itemFromQueryString.Publishing.UnpublishDate = DateUtil.ParseDateTime(this.Unpublish.Value,
                    DateTime.MaxValue);
                foreach (string str2 in Context.ClientPage.ClientRequest.Form.Keys)
                {
                    if ((str2 != null) && str2.StartsWith("pb_", StringComparison.InvariantCulture))
                    {
                        string str3 = ShortID.Decode(StringUtil.Mid(str2, 3));
                        str.Add(str3);
                    }
                }
                itemFromQueryString[FieldIDs.PublishingTargets] = str.ToString();
                itemFromQueryString.Editing.EndEdit();
            }
            Log.Audit(this, "Set publishing targets: {0}, targets: {1}",
                new string[] { AuditFormatter.FormatItem(itemFromQueryString), str.ToString() });
            foreach (string str4 in Context.ClientPage.ClientRequest.Form.Keys)
            {
                if ((str4 != null) && str4.StartsWith("pb_", StringComparison.InvariantCulture))
                {
                    string str5 = ShortID.Decode(StringUtil.Mid(str4, 3));
                    str.Add(str5);
                }
            }
            foreach (Item item2 in itemFromQueryString.Versions.GetVersions())
            {
                bool b =
                    StringUtil.GetString(new string[]
                        {Context.ClientPage.ClientRequest.Form["hide_" + item2.Version.Number]}).Length <= 0;
                DateTimePicker picker = this.Versions.FindControl("validfrom_" + item2.Version.Number) as DateTimePicker;
                DateTimePicker picker2 = this.Versions.FindControl("validto_" + item2.Version.Number) as DateTimePicker;
                Assert.IsNotNull(picker, "Version valid from datetime picker");
                Assert.IsNotNull(picker2, "Version valid to datetime picker");
                DateTime time = DateUtil.IsoDateToDateTime(picker.Value, DateTime.MinValue);
                DateTime time2 = DateUtil.IsoDateToDateTime(picker2.Value, DateTime.MaxValue);

                //Edited by Sitecore Support

                if (((b != item2.Publishing.HideVersion) ||
                     (DateUtil.CompareDatesIgnoringSeconds(DateUtil.ToUniversalTime(time), item2.Publishing.ValidFrom) != 0)) ||
                    (DateUtil.CompareDatesIgnoringSeconds(DateUtil.ToUniversalTime(time2), item2.Publishing.ValidTo) != 0))

                //Edited by Sitecore Support
                {
                    item2.Editing.BeginEdit();
                    item2.Publishing.ValidFrom = time;
                    item2.Publishing.ValidTo = time2;
                    item2.Publishing.HideVersion = b;
                    item2.Editing.EndEdit();
                    Log.Audit(this, "Set publishing valid: {0}, from: {1}, to:{2}, hide: {3}",
                        new string[]
                        {
                            AuditFormatter.FormatItem(item2), time.ToString(), time2.ToString(),
                            MainUtil.BoolToString(b)
                        });
                }
            }
            SheerResponse.SetDialogValue("yes");
            SheerResponse.CloseWindow();
        }



    }
}
