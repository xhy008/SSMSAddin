using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using Microsoft.SqlServer.Management.UI.VSIntegration;
using Microsoft.SqlServer.Management.UI.VSIntegration.Editors;

using System.IO;
using System.Windows.Forms;


namespace MyAddin1
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{            
		}

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;

            executeSqlEvents = _applicationObject.Events.CommandEvents["{52692960-56BC-4989-B5D3-94C47A513E8D}", 1];
            executeSqlEvents.BeforeExecute += ExecuteSqlEventsBeforeExecute;
            executeSqlEvents.AfterExecute += ExecuteSqlEventsAfterExecute;
            

            if (connectMode == ext_ConnectMode.ext_cm_Startup)
			{
				object []contextGUIDS = new object[] { };
				Commands2 commands = (Commands2)_applicationObject.Commands;
				string toolsMenuName = "Tools";

				//Place the command on the tools menu.
				//Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
				Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];

				//Find the Tools command bar on the MenuBar command bar:
				CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
				CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

                CommandBarPopup _ourAddinMenu = menuBarCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing,Type.Missing, toolsControl.Index, true) as CommandBarPopup;
                _ourAddinMenu.Caption = "My Addin Toolbar Item";

                //Create our custom command bar
                CommandBarButton ourCustomToolbarButton;
                CommandBars cmdBars = (CommandBars)_applicationObject.CommandBars;

                ourCustomToolbar = cmdBars.Add("Our add-in Toolbar", MsoBarPosition.msoBarTop, System.Type.Missing, true);

                //This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
                //  just make sure you also update the QueryStatus/Exec method to include the new command names.
                try
				{
					//Add a command to the Commands collection:
					Command command = commands.AddNamedCommand2(_addInInstance, "MyAddin1", "Make A New Query", "Executes the command for MyAddin1", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported+(int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                    Command command2 = commands.AddNamedCommand2(_addInInstance, "MyAddin1ToolWindowForm", "Popup window", "Opens Popup", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

					//Add a control for the command to the tools menu:
					if((command != null) && (toolsPopup != null))
					{
                        command.AddControl(_ourAddinMenu.CommandBar, 1);
					}

                    if ((command2 != null) && (toolsPopup != null))
                    {
                        command2.AddControl(_ourAddinMenu.CommandBar, 2);

                        //Attach our command to the command bar
                        ourCustomToolbarButton = (CommandBarButton)command2.AddControl(ourCustomToolbar,ourCustomToolbar.Controls.Count + 1);
                        ourCustomToolbarButton.Style = MsoButtonStyle.msoButtonIconAndCaption;
                        ourCustomToolbar.Visible = true;
                    }
				}
				catch(System.ArgumentException e)
				{
					//If we are here, then the exception is probably because a command with that name
					//  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
				}                
            }
		}

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
            Commands2 commands = (Commands2)_applicationObject.Commands;
            try
            {
                Command addinCommand = commands.Item("MyAddin1.Connect.MyAddin1");
                addinCommand.Delete();

                Command addinCommand2 = commands.Item("MyAddin1.Connect.MyAddin1ToolWindowForm");
                addinCommand2.Delete();
            }
                
            catch (System.ArgumentException e)
            {
                System.Diagnostics.Debug.Print("Error deleting commands ond isconnection!");
            }

            //Delete our custom toolbar
            if (ourCustomToolbar != null)
            {
                ourCustomToolbar.Delete();
            }
        }

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		
		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
				if(commandName == "MyAddin1.Connect.MyAddin1")
				{
					//status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
					//return;

                    DTE2 dte = ServiceCache.ExtensibilityModel.DTE as DTE2;
                    Document activeDocument = null;

                    activeDocument = dte.ActiveDocument;

                    if (activeDocument == null)
                    {
                        status = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                        commandText = "Enable Our Command";
                    }
                    else
                    {
                        status = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusLatched;
                        commandText = "Our Command Enabled";
                    }
				}
                if (commandName == "MyAddin1.Connect.MyAddin1ToolWindowForm")
                {
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
                    return;
                }
			}
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
		/// <param term='commandName'>The name of the command to execute.</param>
		/// <param term='executeOption'>Describes how the command should be run.</param>
		/// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param term='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
			if(executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
                DTE2 dte = ServiceCache.ExtensibilityModel.DTE as DTE2;
				if(commandName == "MyAddin1.Connect.MyAddin1")
				{
                    
                    Document activeDocument = null;

                    IScriptFactory scriptFactory = ServiceCache.ScriptFactory;
                    if(scriptFactory != null)
                    {
                        scriptFactory.CreateNewBlankScript(ScriptType.Sql);
                        activeDocument = dte.ActiveDocument;
                    }

                    if(activeDocument != null)
                    {
                        TextSelection ts = activeDocument.Selection as TextSelection;
                        ts.Insert("This Query Window was created with our TestAddin.",(int)vsInsertFlags.vsInsertFlagsInsertAtStart);
                    }

                    handled = true;
				}
                if (commandName == "MyAddin1.Connect.MyAddin1ToolWindowForm")
                {
                    Windows2 MyWindow = (Windows2)dte.Windows;

                    Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();

                    object MyControl = null;
                    Window toolWindow = MyWindow.CreateToolWindow2(_addInInstance, asm.Location, "MyAddin1.MyAddinWindow", "MyAddin1.MyAddin1ToolWindowForm", "{5B7F8C1C-65B9-2aca-1Ac3-12AcBbAF21d5}", MyControl);

                    toolWindow.Visible = true;
                    
                    handled = true;
                    /*Assembly a = Assembly.GetExecutingAssembly();
                    object controlObject = null;
                    
                    Windows2 toolWindows = dte.Windows as Windows2;
                    Window2 toolWindow;
                    
                    
                    toolWindow = (Window2)toolWindows.CreateToolWindow2(_addInInstance,a.Location, "MyAddin1.MyAddin1ToolWindowForm ", "", Guid.NewGuid().ToString(), ref controlObject);
                    
                    toolWindow.WindowState = vsWindowState.vsWindowStateNormal;
                    toolWindow.IsFloating = false;
                    toolWindow.Visible = true;
                    
                    handled = true;*/
                }
                else
                {
                    String s = varIn.ToString();
                    string s2 = varOut.ToString();
                }
			}
		}

        private void ExecuteSqlEventsBeforeExecute(string guid, int id, object customin, object customout, ref bool canceldefault)
        {
            try
            {
                MyVars objMyVars = new MyVars();
                string VarValue = objMyVars.MyVar;
                       
                document = ((DTE2)ServiceCache.ExtensibilityModel).ActiveDocument;
                var textDocument = (TextDocument)document.Object("TextDocument");
                

                //selected text to execute
                OriginalQueryText = textDocument.Selection.Text;

                
                if (string.IsNullOrEmpty(OriginalQueryText))
                {
                    //There is no selection- get the whole query
                    EditPoint OriginalStartPoint = textDocument.StartPoint.CreateEditPoint();
                    
                    OriginalQueryText = OriginalStartPoint.GetText(textDocument.EndPoint);
                    OriginalStartPoint.Insert(" " + VarValue + " ");

                    StartPoint = textDocument.StartPoint.CreateEditPoint();
                    EndPoint = textDocument.EndPoint.CreateEditPoint();

                }
                else
                {
                    StartPoint = textDocument.CreateEditPoint(textDocument.Selection.TopPoint);
                    textDocument.Selection.Text = " " + VarValue + " " + OriginalQueryText;
                    EndPoint = textDocument.EndPoint.CreateEditPoint();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ExecuteSqlEventsAfterExecute(string guid, int id, object customin, object customout)
        {
            try
            {                
                
                var textDocument = (TextDocument)document.Object("TextDocument");

                //selected text to execute
                string queryText = textDocument.Selection.Text;


                if (string.IsNullOrEmpty(OriginalQueryText))
                {
                    //No original query text
                }
                else
                {      
                    //Replace with original query              
                    StartPoint.ReplaceText(EndPoint, OriginalQueryText, 8);                    
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private DTE2 _applicationObject;
		private AddIn _addInInstance;
        private CommandBar ourCustomToolbar;

        private CommandEvents executeSqlEvents;

        private string OriginalQueryText;
        private Document document;
        private EditPoint StartPoint;
        private EditPoint EndPoint;
        
    }
}