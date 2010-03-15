<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>SilverBulb</title>
    <style type="text/css">
    html, body {
	    height: 100%;
	    overflow: auto;
    }
    body {
	    padding: 0;
	    margin: 0;
    }
    #silverlightControlHost {
	    height: 100%;
	    text-align:center;
    }
    </style>
    <script type="text/javascript" src="Silverlight.js"></script>
    <script type="text/javascript" >
        function PowerClicked() {
            var control = document.getElementById("silverbulbnes");
            control.Content.ControlPanel.PowerOn();
            document.getElementById("Button1").value = control.Content.ControlPanel.GetPowerStatusText();
        }

        function LoadRom( romname ) {
            var control = document.getElementById("silverbulbnes");
            control.Content.ControlPanel.LoadRom(romname);
        }

    </script>
    <script type="text/javascript">
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
              appSource = sender.getHost().Source;
            }
            
            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
              return;
            }

            var errMsg = "Unhandled Error in Silverlight Application " +  appSource + "\n" ;

            errMsg += "Code: "+ iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {           
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " +  args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }
    </script>
</head>
<body background="ClientBin/blockbacks.png">
    <form id="form1" runat="server" style="height:100%; width:100%" >
    <asp:Image ID="Image1" runat="server" ImageAlign="Middle" 
        ImageUrl="~/ClientBin/mariobg.png"  />
            <input id="Button1" type="button" value="Off" onclick="PowerClicked()" />
            <input id="smb2" type="button" value="1943" onclick="LoadRom('1943.zip')" />
            <input id="Button2" type="button" value="SMB2" onclick="LoadRom('smb2.nes')" />
            <div id="silverlightControlHost">
        <object id="silverbulbnes"
        data="data:application/x-silverlight-2," 
            type="application/x-silverlight-2" 
            style="height:60%; width:100%" 
            >
          <param name="InitParams" value="Cart=testcart.nes,ShowControls=false" />
		  <param name="EnableGPUAcceleration" value="true" />
          <param name="source" value="ClientBin/SilverBulb.xap"/>
		  <param name="onError" value="onSilverlightError" />
		  <param name="background" value="white" />
		  <param name="minRuntimeVersion" value="3.0.40818.0" />
		  <param name="autoUpgrade" value="true" />
		  <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=3.0.40818.0" style="text-decoration:none">
 			  <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" style="border-style:none"/>
		  </a>
	    </object><iframe id="_sl_historyFrame" style="visibility:hidden;height:0px;width:0px;border:0px"></iframe></div>
    </form>
</body>
</html>
