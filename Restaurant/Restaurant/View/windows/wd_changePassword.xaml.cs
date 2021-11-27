using Restaurant.ApiClasses;
using Restaurant.Classes;
using netoaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Restaurant.View.windows
{
    /// <summary>
    /// Interaction logic for wd_changePassword.xaml
    /// </summary>
    public partial class wd_changePassword : Window
    {
        Users userModel = new Users();

        public wd_changePassword()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
                HelpClass.StartAwait(grid_main);

                #region translate

                if (MainWindow.lang.Equals("en"))
                {
                    MainWindow.resourcemanager = new ResourceManager("AdministratorApp.en_file", Assembly.GetExecutingAssembly());
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    MainWindow.resourcemanager = new ResourceManager("AdministratorApp.ar_file", Assembly.GetExecutingAssembly());
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                }

                translate();
                #endregion

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                if (e.Key == Key.Return)
                {
                    Btn_save_Click(null, null);
                }

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        
        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        {//save
            try
            {
                HelpClass.StartAwait(grid_main);

                #region validate 
                bool wrongOldPasswordLength = false, wrongNewPasswordLength = false, wrongConfirmPasswordLength = false;
                //chk empty old password
                if (pb_oldPassword.Password.Equals(""))
                    HelpClass.validateEmpty(pb_oldPassword.Password , p_errorOldPassword);
                else
                {
                    //chk password length
                    wrongOldPasswordLength = chkPasswordLength(pb_oldPassword.Password);
                    if (wrongOldPasswordLength)
                        HelpClass.showPasswordValidate(pb_oldPassword, p_errorOldPassword, tt_errorOldPassword, "trErrorPasswordLengthToolTip");
                    else
                        HelpClass.clearPasswordValidate(pb_oldPassword, p_errorOldPassword);
                }
                //chk empty new password
                if (pb_newPassword.Password.Equals(""))
                    HelpClass.showPasswordValidate(pb_newPassword, p_errorNewPassword, tt_errorNewPassword, "trEmptyPasswordToolTip");
                else
                {
                    //chk password length
                    wrongNewPasswordLength = chkPasswordLength(pb_newPassword.Password);
                    if (wrongNewPasswordLength)
                        HelpClass.showPasswordValidate(pb_newPassword, p_errorNewPassword, tt_errorNewPassword, "trErrorPasswordLengthToolTip");
                    else
                        HelpClass.clearPasswordValidate(pb_newPassword, p_errorNewPassword);
                }
                //chk empty confirm password
                if (pb_confirmPassword.Password.Equals(""))
                    HelpClass.showPasswordValidate(pb_confirmPassword, p_errorConfirmPassword, tt_errorConfirmPassword, "trEmptyPasswordToolTip");
                else
                {
                    //chk password length
                    wrongConfirmPasswordLength = chkPasswordLength(pb_confirmPassword.Password);
                    if (wrongConfirmPasswordLength)
                        HelpClass.showPasswordValidate(pb_confirmPassword, p_errorConfirmPassword, tt_errorConfirmPassword, "trErrorPasswordLengthToolTip");
                    else
                        HelpClass.clearPasswordValidate(pb_confirmPassword, p_errorConfirmPassword);
                }
                #endregion

                #region save
                if ((!pb_oldPassword.Password.Equals("")) && (!wrongOldPasswordLength) &&
                   (!pb_newPassword.Password.Equals("")) && (!wrongNewPasswordLength) &&
                   (!pb_confirmPassword.Password.Equals("")) && (!wrongConfirmPasswordLength))
                {
                    //get password for logined user
                    string loginPassword = MainWindow.userLogin.password;

                    string enteredPassword = Md5Encription.MD5Hash("Inc-m" + pb_oldPassword.Password);

                    if (!loginPassword.Equals(enteredPassword))
                    {
                        HelpClass.showPasswordValidate(pb_oldPassword, p_errorOldPassword, tt_errorOldPassword, "trWrongPassword");
                    }
                    else
                    {
                        HelpClass.clearPasswordValidate(pb_oldPassword, p_errorOldPassword);
                        bool isNewEqualConfirmed = true;
                        if (pb_newPassword.Password.Equals(pb_confirmPassword.Password)) isNewEqualConfirmed = true;
                        else isNewEqualConfirmed = false;

                        if (!isNewEqualConfirmed)
                        {
                            HelpClass.showPasswordValidate(pb_newPassword, p_errorNewPassword, tt_errorNewPassword, "trErrorNewPasswordNotEqualConfirmed");
                            HelpClass.showPasswordValidate(pb_confirmPassword, p_errorConfirmPassword, tt_errorConfirmPassword, "trErrorNewPasswordNotEqualConfirmed");
                        }
                        else
                        {
                            HelpClass.clearPasswordValidate(pb_newPassword, p_errorNewPassword);
                            HelpClass.clearPasswordValidate(pb_confirmPassword, p_errorConfirmPassword);
                            //change password
                            string newPassword = Md5Encription.MD5Hash("Inc-m" + pb_newPassword.Password);
                            MainWindow.userLogin.password = newPassword;
                            int s = await userModel.Save(MainWindow.userLogin);

                            if (s > 0)
                            {
                                if (!Properties.Settings.Default.password.Equals(""))
                                {
                                    Properties.Settings.Default.password = pb_newPassword.Password;
                                    Properties.Settings.Default.Save();
                                }
                                Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopPasswordChanged"), animation: ToasterAnimation.FadeIn);
                                this.Close();
                            }
                            else
                                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                        }
                    }
                }
                #endregion

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Btn_colse_Click(object sender, RoutedEventArgs e)
        {//close
            try
            {
                this.Close();
                // Collect all generations of memory.
                GC.Collect();
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {//closing
            try
            {
                pb_oldPassword.Clear();
                tb_oldPassword.Clear();
                pb_newPassword.Clear();
                tb_newPassword.Clear();
                pb_confirmPassword.Clear();
                tb_confirmPassword.Clear();
                e.Cancel = true;
                this.Visibility = Visibility.Hidden;

                //Collect all generations of memory.
               GC.Collect();
               
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        #region events
        private void Tb_validateEmptyTextChange(object sender, RoutedEventArgs e)
        {
            validateEmptyEvent(sender);
        }

        private void Tb_validateEmptyLostFocus(object sender, RoutedEventArgs e)
        {
            validateEmptyEvent(sender);
        }

       
        private void P_showOldPassword_MouseEnter(object sender, MouseEventArgs e)
        {
            showPassword(tb_oldPassword , pb_oldPassword);
        }

        private void P_showOldPassword_MouseLeave(object sender, MouseEventArgs e)
        {
            hidePassword(tb_oldPassword , pb_oldPassword);
        }
       
        private void P_showNewPassword_MouseEnter(object sender, MouseEventArgs e)
        {
            showPassword(tb_newPassword, pb_newPassword);
        }

        private void P_showNewPassword_MouseLeave(object sender, MouseEventArgs e)
        {
            hidePassword(tb_newPassword, pb_newPassword);
        }

        private void P_showConfirmPassword_MouseEnter(object sender, MouseEventArgs e)
        {
            showPassword(tb_confirmPassword, pb_confirmPassword);
        }

        private void P_showConfirmPassword_MouseLeave(object sender, MouseEventArgs e)
        {
            hidePassword(tb_confirmPassword, pb_confirmPassword);
        }
        #endregion

        #region methods
        private void validateEmptyEvent(object sender)
        {
            try
            {
                string name = sender.GetType().Name;
                validateEmpty(name, sender);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void validateEmpty(string name, object sender)
        {
            try
            {
                if (name == "PasswordBox")
                {
                    if ((sender as PasswordBox).Name == "pb_oldPassword")
                        if (((PasswordBox)sender).Password.Equals(""))
                            HelpClass.showPasswordValidate((PasswordBox)sender, p_errorOldPassword, tt_errorOldPassword, "trEmptyPasswordToolTip");
                        else
                            HelpClass.clearPasswordValidate(pb_oldPassword ,p_errorOldPassword);

                    else if ((sender as PasswordBox).Name == "pb_newPassword")
                        if (((PasswordBox)sender).Password.Equals(""))
                            HelpClass.showPasswordValidate((PasswordBox)sender, p_errorNewPassword, tt_errorNewPassword, "trEmptyPasswordToolTip");
                        else
                            HelpClass.clearPasswordValidate(pb_newPassword , p_errorNewPassword);

                    else if ((sender as PasswordBox).Name == "pb_confirmPassword")
                        if (((PasswordBox)sender).Password.Equals(""))
                            HelpClass.showPasswordValidate((PasswordBox)sender, p_errorConfirmPassword, tt_errorConfirmPassword, "trEmptyPasswordToolTip");
                        else
                            HelpClass.clearPasswordValidate(pb_confirmPassword , p_errorConfirmPassword);
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void translate()
        {
            txt_title.Text = MainWindow.resourcemanager.GetString("trChangePassword");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(pb_oldPassword, MainWindow.resourcemanager.GetString("trOldPasswordHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(pb_newPassword, MainWindow.resourcemanager.GetString("trNewPasswordHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(pb_confirmPassword, MainWindow.resourcemanager.GetString("trConfirmedPasswordHint"));

            btn_save.Content = MainWindow.resourcemanager.GetString("trSave");

            tt_oldPassword.Content = MainWindow.resourcemanager.GetString("trOldPassword");
            tt_newPassword.Content = MainWindow.resourcemanager.GetString("trNewPassword");
            tt_confirmPassword.Content = MainWindow.resourcemanager.GetString("trConfirmedPassword");
        }
        private void showPassword(TextBox tb, PasswordBox pb)
        {
            try
            {
                tb.Text = pb.Password;
                tb.Visibility = Visibility.Visible;
                pb.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void hidePassword(TextBox tb, PasswordBox pb)
        {
            try
            {
                tb.Visibility = Visibility.Collapsed;
                pb.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private bool chkPasswordLength(string password)
        {
            bool b = false;
            if (password.Length < 6)
                b = true;
            return b;
        }
        #endregion
    }
}
