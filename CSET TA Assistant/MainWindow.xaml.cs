using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using word = Microsoft.Office.Interop.Word;

namespace CSET_TA_Assistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string magicspace = "_____ ";
        const string magicx = "__✓__ ";
        const string strCourseName = "CST211";      //needs to be put in combo box on gui
        const bool WatchWordWork = false;           //change this to watch automation of word
        int howLate = 0;
        Brush textBrush = null;
        object missing = System.Reflection.Missing.Value;

        public MainWindow()
        {
            InitializeComponent();
            cmbProfessor.Items.Add("Todd Breedlove");
            cmbProfessor.SelectedIndex = 0;
            cmbProfessor.IsEnabled = false;
            textBrush = txtStudent.BorderBrush;
        }

        private void btnClear(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Clear everything without saving?", "Be careful!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
                Clear();

        }

        private void Clear()
        {
            foreach (var control in this.GetChildren())
            {
                if (control is CheckBox)
                {
                    CheckBox chk = control as CheckBox;
                    if (chk.Name != "chkAppend")
                        chk.IsChecked = false;
                }
                else if (control is TextBox)
                {
                    TextBox txt = control as TextBox;
                    if (txt.Text != cmbProfessor.Text && txt.Text != txtAssignment.Text && txt != txtPath)
                        txt.Text = "";
                }
            }
        }

        private bool CheckForm()
        {
            bool pass = true;
            if (txtStudent.Text.Length <= 0)
            {
                pass = false;
                txtStudent.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
                txtStudent.BorderBrush = textBrush;

            if (txtAssignment.Text.Length <= 0)
            {
                txtAssignment.BorderBrush = new SolidColorBrush(Colors.Red);
                pass = false;
            }
            else
                txtAssignment.BorderBrush = textBrush;

            if (!Directory.Exists(txtPath.Text) || !File.Exists(txtPath.Text + "\\GradingTemplate.doc"))
            {
                    MessageBox.Show("Template not found: GradingTemplate.doc\nPlace inside grading folder.", "Missing GradingTemplate.doc", MessageBoxButton.OK, MessageBoxImage.Warning);

                txtPath.BorderBrush = new SolidColorBrush(Colors.Red);
                pass = false;
            }
            else
                txtPath.BorderBrush = textBrush;
            return pass;
        }


        private void btnCreateDoc_Click(object sender, RoutedEventArgs e)
        {
            if (CheckForm() == false)
                return;

            bool failed = false;

            btnCreateDoc.IsEnabled = false;
            btnCreateDoc.Content = "Working";
            //Everything has to be an object(COM) to pass to word.
            object strFileName = txtPath.Text + "\\" + txtAssignment.Text + " - " + txtStudent.Text + ".doc";
            try
            {
                File.Copy(txtPath.Text + "\\GradingTemplate.doc", (string)strFileName, true);
            }
            catch
            {
                failed = true;
                MessageBox.Show("Error can't access " + strFileName);
            }

            if (failed == false)
            {
                word.Application wordApp = new word.Application();
                wordApp.Visible = WatchWordWork;
                wordApp.WindowState = word.WdWindowState.wdWindowStateNormal;

                object missing = System.Reflection.Missing.Value;
                object readOnly = false;
                object isVisible = true;

                word.Document doc = wordApp.Documents.Open(ref strFileName, ref missing, ref readOnly, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref isVisible);
                // Activate the document so it shows up in front  
                doc.Activate();

                //I don't know how to search header of word doc. Works in old word files, not docx
                //SearchReplace("#1", strCourseName, wordApp);
                //SearchReplace("#2", cmbProfessor.Text, wordApp);
                SearchReplace("#3", txtStudent.Text, wordApp);
                SearchReplace("#4", txtAssignment.Text, wordApp);

                //This magic lets me use the checkbox name as the ms word field name
                //You can just add a checkbox to the form and a spot on the template
                //and it will automatically detect it with no code changes.
                foreach (var control in this.GetChildren())
                {
                    if (control is CheckBox)
                    {
                        CheckBox chkBox = control as CheckBox;
                        if (chkBox.IsChecked == true && chkBox != chkAppend)
                        {
                            string newText = chkBox.Content.ToString();

                            //late checkbox is special
                            if (chkBox == chkLate)
                                newText += " " + howLate.ToString() + " days";

                            SearchReplace(magicspace + chkBox.Content.ToString(), magicx + newText, wordApp);
                        }
                    }
                }

                //this is a terrible way to do this...
                //but I'm short on time, I have Physics to do.
                SearchReplace("#5", txtMemoryLeak.Text, wordApp);
                SearchReplace("#6", txtIncorrectStatement.Text, wordApp);
                SearchReplace("#7", txtRedundantCode.Text, wordApp);
                SearchReplace("#8", txtBasemember.Text, wordApp);
                SearchReplace("#9", txtDestructor.Text, wordApp);
                SearchReplace("#10", txtOperatorEq.Text, wordApp);
                SearchReplace("#11", txtCopyConstructor.Text, wordApp);
                SearchReplace("#12", txtConstructor.Text, wordApp);
                SearchReplace("#13", txtNotSeperatehcpp.Text, wordApp);

                //commenting section
                SearchAndType("#14", txtMissingFunctionality.Text.ToString(), wordApp);
                SearchAndType("#15", txtRuntimeCrash.Text.ToString(), wordApp);
                SearchAndType("#16", txtLogicError.Text.ToString(), wordApp);
                SearchAndType("#17", txtComment.Text.ToString(), wordApp);

                //insert the comments at the end.
                //wordApp.Selection.EndKey(word.WdUnits.wdStory, ref missing);
                //wordApp.Selection.TypeText("\r\n\r\nComments: " + txtComment.Text);

                doc.Save();

                //Merge to one doc
                if (chkAppend.IsChecked == true)
                {
                    doc.Close();
                    object masterFile = txtPath.Text + "\\" + strCourseName + " - " + txtAssignment.Text + ".doc";

                    if (!File.Exists((string)masterFile))
                    {
                        File.Copy((string)strFileName, (string)masterFile);
                    }
                    else
                    {
                        word.Document d = wordApp.Documents.Open(ref masterFile, ref missing, ref readOnly, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref isVisible);
                        word.Selection selection = wordApp.Selection;

                        //should put this in function.
                        //Go to end of page.
                        Object toWhat = word.WdGoToItem.wdGoToLine;
                        Object toWhich = word.WdGoToDirection.wdGoToLast;
                        wordApp.Selection.GoTo(toWhat, toWhich, ref missing, ref missing);
                        wordApp.Selection.EndKey(word.WdUnits.wdStory, ref missing);

                        object pageBreak = word.WdBreakType.wdPageBreak;
                        selection.InsertBreak(ref pageBreak);

                        selection.InsertFile((string)strFileName);
                        d.Save();

                        d.Close();
                        //delete old student templated file.
                        File.Delete((string)strFileName);
                    }

                }

                if(!WatchWordWork)
                    wordApp.Quit();
            }

            Clear();
            btnCreateDoc.Content = "Create Doc";
            btnCreateDoc.IsEnabled = true;
        }

        //This is different. It supports replacing with insert of \r\n.
        private void SearchAndType(string SearchFor, string ToType, word.Application wordApp)
        {
            word.Find findObj = wordApp.Selection.Find;
            findObj.ClearFormatting();
            findObj.Text = SearchFor;
            findObj.Replacement.ClearFormatting();
            findObj.Replacement.Text = "";


            object replace = word.WdReplace.wdReplaceOne;
            findObj.Execute(ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing,
                ref replace, ref missing, ref missing, ref missing, ref missing);

            wordApp.Selection.TypeText(ToType);
        }

        //This finds and replaces, but does not support replacing with \r\n inside a string.
        private void SearchReplace(string SearchFor, string ReplaceWith, word.Application wordApp)
        {
            word.Find findObj = wordApp.Selection.Find;
            findObj.ClearFormatting();
            findObj.Text = SearchFor;
            findObj.Replacement.ClearFormatting();
            findObj.Replacement.Text = ReplaceWith;

            object replace = word.WdReplace.wdReplaceOne;
            findObj.Execute(ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing,
                ref replace, ref missing, ref missing, ref missing, ref missing);

            //same thing as clicking away from a selection
            wordApp.Selection.Collapse();

            //put the cursor at start of document or find won't work.
            Object toWhat = word.WdGoToItem.wdGoToLine;
            Object toWhich = word.WdGoToDirection.wdGoToFirst;
            wordApp.Selection.GoTo(toWhat, toWhich, ref missing, ref missing);
        }

        private void btnBrowseFiles_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    txtPath.Text = fbd.SelectedPath;
            }
        }

        private void chkLate_Checked(object sender, RoutedEventArgs e)
        {
            if (chkLate.IsChecked == true)
            {
                InputDialog dialog = new InputDialog();
                dialog.Text = "How many Days?";
                dialog.ShowDialog(); //blocks
                int.TryParse(dialog.value, out howLate);
            }
        }
    }
}
