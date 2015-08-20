using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyAddin1
{  
    

    public partial class MyAddinWindow : UserControl
    {
        private int VarCount = 1;
        private int VarBoxBottom;

        private List<TextBox> liVariableBoxes = new List<TextBox>();
        private List<TextBox> liTypeBoxes = new List<TextBox>();
        private List<TextBox> liValueBoxes = new List<TextBox>();

        private MyVars objMyVars = new MyVars();

        public MyAddinWindow()
        {
            InitializeComponent();            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Saves variable list          
            if (liVariableBoxes.Count < 1 )
            {
                objMyVars.MyVar = "DECLARE " + txtVar1.Text + " " + txtType1.Text + " SET " + txtVar1.Text + " = " + txtVal1.Text;
            }
            else
            {
                string DeclareVars = "DECLARE " + txtVar1.Text + " " + txtType1.Text;
                string DeclareVal = "SET " + txtVar1.Text + " = " + txtVal1.Text;

                for (int i = 0;i<liVariableBoxes.Count;i++)
                {
                    if (liVariableBoxes.ElementAt(i).Text != "")
                    {
                        DeclareVars += ", " + liVariableBoxes.ElementAt(i).Text + " " + liTypeBoxes.ElementAt(i).Text + " ";
                        DeclareVal += " SET " + liVariableBoxes.ElementAt(i).Text + " = " + liValueBoxes.ElementAt(i).Text + " ";
                    }
                    
                }
                objMyVars.MyVar = DeclareVars + DeclareVal;
            }

            objMyVars.enabled = chkEnabled.Checked;            
            
        }

        private void btnAddVar_Click(object sender, EventArgs e)
        {
            //Adds a new variable row
            VarBoxBottom = txtVar1.Bottom;

            VarCount++;
            //Add Variable Name textbox
            TextBox NewTxt = new TextBox();
            NewTxt.Name = "txtVar" + VarCount.ToString();         
            NewTxt.Location = new System.Drawing.Point(txtVar1.Left, VarBoxBottom + (20 * VarCount));
            NewTxt.Width = txtVal1.Width;
            this.Controls.Add(NewTxt);
            liVariableBoxes.Add(NewTxt);

            //Add Type Textbox
            TextBox NewTxtType = new TextBox();
            NewTxtType.Name = "txtType" + VarCount.ToString();
            NewTxtType.Location = new System.Drawing.Point(txtType1.Left, VarBoxBottom + (20 * VarCount));
            NewTxtType.Width = txtVal1.Width;
            this.Controls.Add(NewTxtType);
            liTypeBoxes.Add(NewTxtType);

            //Add Value textbox
            TextBox NewTxtValue = new TextBox();
            NewTxtValue.Name = "txtVal" + VarCount.ToString();
            NewTxtValue.Location = new System.Drawing.Point(txtVal1.Left, VarBoxBottom + (20 * VarCount));
            NewTxtValue.Width = txtVal1.Width;
            this.Controls.Add(NewTxtValue);
            liValueBoxes.Add(NewTxtValue);

            //Move save button
            button1.Location = new System.Drawing.Point(button1.Left, button1.Top + 20 * VarCount);            
        }
    }

    public class MyVars
    {
        private static string mMyVar;
        private static bool menabled;

        public MyVars()
        {
        }

        public string MyVar
        {
            get
            {
                return mMyVar;
            }
            set
            {
                mMyVar = value;
            }

        }

        public bool enabled
        {
            get
            {
                return menabled;
            }
            set
            {
                menabled = value;
            }
        }

    }

}
