﻿using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using XrmToolBox.Forms;

namespace XrmToolBox.New
{
    partial class NewForm
    {
        private ComponentResourceManager resources = new ComponentResourceManager(typeof(NewForm));

        #region Events

        private void displayXrmToolBoxHelpToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Process.Start("https://www.xrmtoolbox.com/documentation/");
        }

        private void donateDollarPluginMenuItem_Click(object sender, System.EventArgs e)
        {
            if (((PluginForm)dpMain.ActiveContent).Control is IPayPalPlugin payPal)
            {
                Donate("EN", "USD", payPal.EmailAccount, payPal.DonationDescription);
            }
        }

        private void donateEuroPluginMenuItem_Click(object sender, System.EventArgs e)
        {
            if (((PluginForm)dpMain.ActiveContent).Control is IPayPalPlugin payPal)
            {
                Donate("EN", "EUR", payPal.EmailAccount, payPal.DonationDescription);
            }
        }

        private void donateGbpPluginMenuItem_Click(object sender, System.EventArgs e)
        {
            if (((PluginForm)dpMain.ActiveContent).Control is IPayPalPlugin payPal)
            {
                Donate("EN", "GBP", payPal.EmailAccount, payPal.DonationDescription);
            }
        }

        private void donateInEuroToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Donate("EN", "EUR", "tanguy92@hotmail.com", "Donation for MSCRM Tools - XrmToolBox");
        }

        private void donateInGBPToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Donate("EN", "GBP", "tanguy92@hotmail.com", "Donation for MSCRM Tools - XrmToolBox");
        }

        private void donateInUSDollarsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Donate("EN", "USD", "tanguy92@hotmail.com", "Donation for MSCRM Tools - XrmToolBox");
        }

        #endregion Events

        #region Prepare Community items

        private void AssignPayPalMenuItems(ToolStripItemCollection dropDownItems)
        {
            dropDownItems.AddRange(new ToolStripItem[]
            {
                tsmiDonateUsdXrmToolBox,
                tsmiDonateEurXrmToolBox,
                tsmiDonateGbpXrmToolBox
            });
        }

        private void feedbackToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            GithubXrmToolBoxMenuItem_Click(sender, e);
        }

        private string GetAdditionalInfo()
        {
            var additionalInfo = "";
            if (connectionDetail != null)
            {
                additionalInfo +=
                    $"-%20Deployment:%20{(connectionDetail.WebApplicationUrl.ToLower().Contains("dynamics.com") ? "Online" : "OnPremise")}%0A";

                additionalInfo += $"-%20DB%20Version:%20{connectionDetail.OrganizationVersion}%0A";
            }

            additionalInfo += $"-%20XTB%20Version:%20{Assembly.GetExecutingAssembly().GetName().Version}%0A";
            additionalInfo = "?body=[Write your comment/feedback/issue here]%0A%0A---%0A" + additionalInfo;

            return additionalInfo;
        }

        private void githubPluginMenuItem_Click(object sender, System.EventArgs e)
        {
            var url = GetGithubBaseUrl("issues/new");

            var additionalInfo = GetAdditionalInfo();

            if (currentContent is PluginForm pf)
            {
                additionalInfo += $"-%20Tool%20Version:%20{pf.Control.GetType().Assembly.GetName().Version}%0A";
            }

            Process.Start(url + additionalInfo);
        }

        private void GithubXrmToolBoxMenuItem_Click(object sender, System.EventArgs e)
        {
            var url = "https://github.com/MscrmTools/XrmToolBox/issues/new";
            var additionalInfo = GetAdditionalInfo();

            Process.Start(url + additionalInfo);
        }

        private void HelpSelectedPluginToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Process.Start(GetHelpUrl());
        }

        private void helpToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (!(dpMain.ActiveContent is PluginForm)) // Home Screen
                displayXrmToolBoxHelpToolStripMenuItem_Click(sender, e);
        }

        private void ProcessMenuItemsForPlugin()
        {
            if (!(dpMain.ActiveContent is PluginForm)) // Home Screen
            {
                tsmiFeedbackXrmToolBox.Visible = false;
                tsmiFeedbackSelectedPlugin.Visible = false;
                tsmiDonateXrmToolBox.Visible = false;
                tsmiDonateSelectedPlugin.Visible = false;
                tsmiHelpXrmToolBox.Visible = false;
                tsmiHelpSelectedPlugin.Visible = false;
                tsmiAboutXrmToolBox.Visible = false;
                tsmiAboutSelectedPlugin.Visible = false;
                AssignPayPalMenuItems(tsmiDonate.DropDownItems);
                return;
            }

            var pluginName = ((PluginForm)dpMain.ActiveContent).PluginTitle;

            if (((PluginForm)dpMain.ActiveContent).Control is IHelpPlugin help)
            {
                tsmiHelpXrmToolBox.Visible = true;
                tsmiHelpSelectedPlugin.Visible = true;
                tsmiHelpSelectedPlugin.Text =
                    string.Format(tsmiHelpSelectedPlugin.Tag.ToString(), pluginName);
                tsmiHelpSelectedPlugin.Image = (help as PluginControlBase)?.TabIcon;
                tsmiHelp.Click -= displayXrmToolBoxHelpToolStripMenuItem_Click;
            }
            else
            {
                tsmiHelpXrmToolBox.Visible = false;
                tsmiHelpSelectedPlugin.Visible = false;
                tsmiHelp.Click -= displayXrmToolBoxHelpToolStripMenuItem_Click;
                tsmiHelp.Click += displayXrmToolBoxHelpToolStripMenuItem_Click;
            }

            if (((PluginForm)dpMain.ActiveContent).Control is IPayPalPlugin paypal)
            {
                tsmiDonateXrmToolBox.Visible = true;
                tsmiDonateSelectedPlugin.Visible = true;
                tsmiDonateSelectedPlugin.Text = pluginName;
                tsmiDonateSelectedPlugin.Image = (paypal as PluginControlBase)?.TabIcon;
                AssignPayPalMenuItems(tsmiDonateXrmToolBox.DropDownItems);
            }
            else
            {
                tsmiDonateXrmToolBox.Visible = false;
                tsmiDonateSelectedPlugin.Visible = false;
                AssignPayPalMenuItems(tsmiDonate.DropDownItems);
            }

            if (((PluginForm)dpMain.ActiveContent).Control is IGitHubPlugin github)
            {
                tsmiFeedbackXrmToolBox.Visible = true;
                tsmiFeedbackSelectedPlugin.Visible = true;
                tsmiFeedbackSelectedPlugin.Text = pluginName;
                tsmiFeedbackSelectedPlugin.Image = (github as PluginControlBase)?.TabIcon;
                tsmiFeedback.Click -= feedbackToolStripMenuItem_Click;
            }
            else
            {
                tsmiFeedbackXrmToolBox.Visible = false;
                tsmiFeedbackSelectedPlugin.Visible = false;
                tsmiFeedback.Click -= feedbackToolStripMenuItem_Click;
                tsmiFeedback.Click += feedbackToolStripMenuItem_Click;
            }

            if (((PluginForm)dpMain.ActiveContent).Control is IAboutPlugin aboutPlugin)
            {
                tsmiAboutXrmToolBox.Visible = true;
                tsmiAboutSelectedPlugin.Visible = true;
                tsmiAboutSelectedPlugin.Text = pluginName;
                tsmiAboutSelectedPlugin.Image = (aboutPlugin as PluginControlBase)?.TabIcon;
                tsmiAbout.Click -= tsmiAbout_Click;
            }
            else
            {
                tsmiAboutXrmToolBox.Visible = false;
                tsmiAboutSelectedPlugin.Visible = false;
                tsmiAbout.Click -= tsmiAbout_Click;
                tsmiAbout.Click += tsmiAbout_Click;
            }
        }

        private void ProcessMenuItemsForPlugin2()
        {
            var isPluginForm = dpMain.ActiveContent is PluginForm;
            tsmiPluginAbout.Visible = false;
            tsmiPluginDonate.Visible = false;
            tsmiPluginFeedback.Visible = false;
            tsmiPluginHelp.Visible = false;
            tssHelp.Visible = false;
            tssFeedback.Visible = isPluginForm;
            //((System.Drawing.Image)(resources.GetObject("tsmiHelp.Image")))
            if (!isPluginForm)
            {
                tsmiXtbHelp.Text = @"Help";
                tsmiXtbHelp.Image = null;
                tsmiXtbDonate.Text = @"Donate";
                tsmiXtbDonate.Image = null;
                tsmiXtbFeedback.Text = @"Feedback";
                tsmiXtbFeedback.Image = null;
                tsmiXtbAbout.Text = @"About";
                tsmiXtbAbout.Image = null;

                return;
            }

            tsmiXtbHelp.Text = @"Help for XrmToolBox";
            tsmiXtbHelp.Image = (Image)resources.GetObject("tsmiHelpXrmToolBox.Image");
            tsmiXtbDonate.Text = @"Donate for XrmToolBox";
            tsmiXtbDonate.Image = (Image)resources.GetObject("tsmiHelpXrmToolBox.Image");
            tsmiXtbFeedback.Text = @"Feedback for XrmToolBox";
            tsmiXtbFeedback.Image = (Image)resources.GetObject("tsmiHelpXrmToolBox.Image");
            tsmiXtbAbout.Text = @"About XrmToolBox";
            tsmiXtbAbout.Image = (Image)resources.GetObject("tsmiHelpXrmToolBox.Image");

            var currentPluginForm = (PluginForm)dpMain.ActiveContent;

            if (currentPluginForm.Control is IHelpPlugin help)
            {
                tsmiPluginHelp.Visible = true;
                tssHelp.Visible = true;
                tsmiPluginHelp.Text = $@"Help for {currentPluginForm.PluginTitle}";
                tsmiPluginHelp.Image = (help as PluginControlBase)?.TabIcon;
            }
            else
            {
                tsmiXtbHelp.Image = null;
            }

            if (currentPluginForm.Control is IPayPalPlugin paypal)
            {
                tsmiPluginDonate.Visible = true;
                tsmiPluginDonate.Text = $@"Donate for {currentPluginForm.PluginTitle}";
                tsmiPluginDonate.Image = (paypal as PluginControlBase)?.TabIcon;
            }
            else
            {
                tsmiXtbDonate.Image = null;
            }

            if (currentPluginForm.Control is IGitHubPlugin github)
            {
                tsmiPluginFeedback.Visible = true;
                tsmiPluginFeedback.Text = $@"Feedback for {currentPluginForm.PluginTitle}";
                tsmiPluginFeedback.Image = (github as PluginControlBase)?.TabIcon;
            }
            else
            {
                tsmiXtbFeedback.Image = null;
            }

            if (currentPluginForm.Control is IAboutPlugin aboutPlugin)
            {
                tsmiPluginAbout.Visible = true;
                tsmiPluginAbout.Text = $@"About {currentPluginForm.PluginTitle}";
                tsmiPluginAbout.Image = (aboutPlugin as PluginControlBase)?.TabIcon;
            }
            else
            {
                tsmiXtbAbout.Image = null;
            }
        }

        #endregion Prepare Community items

        private void Donate(string language, string currency, string emailAccount, string description)
        {
            var url =
               string.Format(
                   "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business={0}&lc={1}&item_name={2}&currency_code={3}&bn=PP%2dDonationsBF",
                   emailAccount,
                   language,
                   HttpUtility.UrlEncode(description),
                   currency);

            Process.Start(url);
        }

        private string GetGithubBaseUrl(string page)
        {
            var plugin = (IGitHubPlugin)((PluginForm)dpMain.ActiveContent).Control;
            return $"https://github.com/{plugin.UserName}/{plugin.RepositoryName}/{page}";
        }

        private string GetHelpUrl()
        {
            var plugin = (IHelpPlugin)((PluginForm)dpMain.ActiveContent).Control;
            return plugin.HelpUrl;
        }

        private void tsmiAbout_Click(object sender, System.EventArgs e)
        {
            var aForm = new WelcomeDialog(false) { StartPosition = FormStartPosition.CenterParent };
            aForm.ShowDialog(this);
        }

        private void tsmiAboutSelectedPlugin_Click(object sender, System.EventArgs e)
        {
            var plugin = (IAboutPlugin)((PluginForm)dpMain.ActiveContent).Control;
            plugin.ShowAboutDialog();
        }

        private void tsmiAboutXrmToolBox_Click(object sender, System.EventArgs e)
        {
            tsmiAbout_Click(sender, e);
        }
    }
}