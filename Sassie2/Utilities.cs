using Microsoft.ApplicationBlocks.Data;
using Sassie2;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Web;
using System.Xml;

[Serializable]
public struct ErrInfo
{
	public string moduleName;
	public SeverityType severityType;
	public string sqlCall;
	public string lineOfBusiness;
}
namespace Sassie2
{
    public enum SeverityType : short
	{
		Information = 0,
		Warning = 1,
		RuntimeError = 2,
		DbError = 3,
		DbJobError = 4,
		CriticalError = 10
	}

	public class Utilities
	{
		private string _sEmailTo;
		private string _sEmailCC;
		private string _sEmailBCC;
		private string _sEmailFrom;
		private ErrInfo ErrorInfo;

		public string SendPasswordViaEmail(string _sEmail, string _sUserName, string _sPassword)
		{
			string _sMessage = null;

			_sMessage = "Dear " + _sUserName + "," + Environment.NewLine + "Thank you for your inquiry received on " + DateTime.Now.ToString("D") + " " + DateTime.Now.ToString("T") + ". Your password is: " + _sPassword + "." + Environment.NewLine + "If you need to contact us in reference to this " + "matter please call True360 Helpdesk support group at 1-877-737-6157 or send an email " + "to asi.helpdesk@acvauctions.com.";
			try
			{
				SendEmail(_sEmail, "asi.helpdesk@acvauctions.com", "User Password for True360 Website", _sMessage);
			}
			catch (Exception E)
			{
				throw E;
			}
			return _sMessage;
		}

		public void SendEmail(string _sTo, string _sFrom, string _sSubject, string _sMessage, string _sCC = "", string _sBCC = "", string _sAttachment = "")
		{
			MailMessage _oMessage;
			SmtpClient _oSmtp = new SmtpClient();
			string _sSendURI = null;
			string _sSendValue = null;
			string _sPickupURI = null;
			string _sPickupValue = null;
			string _sServer = null;

			////Obtain SMTP server settings from SGSNADb.xml
			getSMTPConfig(ref _sSendURI, ref _sSendValue, ref _sPickupURI, ref _sPickupValue, ref _sServer);

			_oMessage = new MailMessage();
			//Mail.SmtpMail.SmtpServer = Environment.MachineName
			var _with1 = _oMessage;
			_with1.To.Add(new MailAddress(_sTo));
			_with1.CC.Add(new MailAddress(_sCC));
			_with1.Bcc.Add(new MailAddress(_sBCC));
			_with1.From = new MailAddress(_sFrom);
			_with1.Subject = _sSubject;
			_with1.Priority = MailPriority.High;
			_with1.IsBodyHtml = _oMessage.IsBodyHtml;
			_with1.Body = _sMessage;

			_oSmtp.PickupDirectoryLocation = _sPickupValue;

			//_with1.Fields(_sSendURI) = Convert.ToInt32(_sSendValue);
			//_with1.Fields(_sPickupURI) = _sPickupValue;

			if (!string.IsNullOrEmpty(_sServer.Trim()))
			{
				_oSmtp.Host = _sServer;
			}

			try
			{
				//_oSmtp.SmtpServer = ""

				//Attach File
				if (_sAttachment.Length > 0)
				{
					if (File.Exists(_sAttachment))
					{
						Attachment mAttachment = new Attachment(_sAttachment);
						_oMessage.Attachments.Add(mAttachment);
					}
				}

				_oSmtp.Send(_oMessage);
			}
			catch (Exception E)
			{
				throw E;
			}
			finally
			{
				_oMessage = null;
				_oSmtp = null;
			}
		}

		public string GetMainPageName(string _sSrc)
		{
			string functionReturnValue = null;
			MSXML2.DOMDocument _xDoc = default(MSXML2.DOMDocument);
			string XMLSource = null;
			MSXML2.IXMLDOMNode _Root = default(MSXML2.IXMLDOMNode);
			MSXML2.IXMLDOMNode _Child = default(MSXML2.IXMLDOMNode);
			int _Indx = 0;

			_xDoc = new MSXML2.DOMDocument();
			XMLSource = "SGSNA.xml";

			_xDoc.load(XMLSource);
			_Root = _xDoc.getElementsByTagName("appSettings")[0];
			for (_Indx = 0; _Indx <= _Root.childNodes.length - 1; _Indx++)
			{
				_Child = _Root.childNodes[_Indx];
				switch (_Child.nodeName)
				{
					case "Remarketing":
						if (_sSrc == "REMARK")
							functionReturnValue = _Child.text;
						break;
					case "Map":
						if (_sSrc == "MAP")
							functionReturnValue = _Child.text;
						break;
				}
			}
			_xDoc = null;
			_Root = null;
			_Child = null;
			return functionReturnValue;
		}

		public string GetConnectionString(string sDatabase)
		{
			MSXML2.DOMDocument _xDoc;
			string XMLSource = null;
			MSXML2.IXMLDOMNode _Root;
			MSXML2.IXMLDOMNode _Child;

			var server = string.Empty;
			var database = string.Empty;
			var user = string.Empty;
			var password = string.Empty;

			if (sDatabase == "VEHICLE" || sDatabase == "USER" || sDatabase == "VRE")
			{
				var xmlDoc = new XmlDocument();
				xmlDoc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExternalReferences", "Settings.xml"));

				if (sDatabase == "VEHICLE" || sDatabase == "USER")
				{
					sDatabase = xmlDoc.GetElementsByTagName("VEHICLEDATABASE").Item(0).InnerText;
				}
				else if (sDatabase == "VRE")
				{
					sDatabase = xmlDoc.GetElementsByTagName("VREDATABASE").Item(0).InnerText;
				}
			}

			_xDoc = new MSXML2.DOMDocument();
			_xDoc.load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExternalReferences", "SGSNADB.xml"));
			_Root = _xDoc.getElementsByTagName(sDatabase)[0];
			for (var i = 0; i <= _Root.childNodes.length - 1; i++)
			{
				_Child = _Root.childNodes[i];
				switch (_Child.nodeName.ToUpper())
				{
					case "USERID":
						user = _Child.text;
						break;
					case "PASSWORD":
						password = _Child.text;
						break;
					case "SERVER":
						server = _Child.text;
						break;
					case "DATABASE":
						database = _Child.text;
						break;
				}
			}

			return BuildConnString(server, database, user, password);
		}

		private string BuildConnString(string sServer, string sDatabase, string sdbUser, string sdbPassword)
		{
			using (var crypto = new SGSCryptoGraphy.SGSCrypto())
			{
				return $"Server=acv-asi-production.cylznspjfdxr.us-east-1.rds.amazonaws.com;Database={crypto.DecryptMessage(sDatabase)};User id={crypto.DecryptMessage(sdbUser)};password={crypto.DecryptMessage(sdbPassword)};";
				//#if DEBUG

				//                return $"Server={crypto.DecryptMessage(sServer)}.rds.amazonaws.com;Database={crypto.DecryptMessage(sDatabase)};User id={crypto.DecryptMessage(sdbUser)};password={crypto.DecryptMessage(sdbPassword)};";
				//#else
				//return $"Server={crypto.DecryptMessage(sServer)};Database={crypto.DecryptMessage(sDatabase)};User id={crypto.DecryptMessage(sdbUser)};password={crypto.DecryptMessage(sdbPassword)};";
				//#endif
			}
        }

		public DataSet GetRefTabDesc(string sType, string RefTab_Value)
		{
			DataSet functionReturnValue = null;
			string _sConnString = null;
			string _sSql = null;
			SqlParameter[] _oParameter = new SqlParameter[2];
			dynamic _RefTab_ID = null;

			try
			{
				switch (sType)
				{
					case "FILE":
						_RefTab_ID = 72;
						break;
					case "STATUS":
						_RefTab_ID = 73;
						break;
					case "ACTIVITY":
						_RefTab_ID = 74;
						break;
				}

				_sConnString = GetConnectionString("VEHICLE");
				_sSql = "usp_REM_Get_RefTabDesc";
				_oParameter[0] = new SqlParameter("@RefTabDet_ID", _RefTab_ID);
				_oParameter[1] = new SqlParameter("@RefTabDet_Value", RefTab_Value);

				functionReturnValue = SqlHelper.ExecuteDataset(_sConnString, CommandType.StoredProcedure, _sSql, _oParameter);
			}
			catch (Exception e)
			{
				SaveSqlCall(SeverityType.RuntimeError, "Remarketing", "SGSUtility::Utilities::GetRefTabDesc", _sSql, _oParameter);
				throw e;
			}
			finally
			{
				_RefTab_ID = null;
				_oParameter = null;
			}
			return functionReturnValue;

		}

		public DataSet GetVehicleActivity(int iAssignment, string _sPage)
		{
			DataSet functionReturnValue = null;
			SqlParameter[] _oParameter = new SqlParameter[2];
			string _sConnString = null;
			string _sSql = null;
			int _RefTab_ID = 0;

			try
			{
				_sConnString = GetConnectionString("VEHICLE");
				_sSql = "usp_REM_Get_VehicleActivity";

				_oParameter[0] = new SqlParameter("@Assignment_ID", iAssignment);
				_oParameter[1] = new SqlParameter("@Page", _sPage);

				functionReturnValue = SqlHelper.ExecuteDataset(_sConnString, CommandType.StoredProcedure, _sSql, _oParameter);
			}
			catch (Exception e)
			{
				SaveSqlCall(SeverityType.RuntimeError, "Remarketing", "SGSUtility::Utilities::GetVehicleActivity", _sSql, _oParameter);
				throw e;
			}
			finally
			{
				_oParameter = null;
			}
			return functionReturnValue;
		}

		public SqlDataReader GetVehicleInspectionInfo(int iAssignment_id, int iInspection_id, int iVehicle_ID)
		{
			SqlDataReader functionReturnValue = default(SqlDataReader);
			SqlParameter[] _oParameter = new SqlParameter[3];
			string _sConnString = null;
			string _sSql = null;

			try
			{
				_sConnString = GetConnectionString("VEHICLE");
				_sSql = "usp_REM_Get_VehicleInspection";

				_oParameter[0] = new SqlParameter("@Assignment_ID", iAssignment_id);
				_oParameter[1] = new SqlParameter("@Inspection_ID", iInspection_id);
				_oParameter[2] = new SqlParameter("@Vehicle_ID", iVehicle_ID);

				functionReturnValue = SqlHelper.ExecuteReader(_sConnString, CommandType.StoredProcedure, _sSql, _oParameter);
			}
			catch (Exception e)
			{
				SaveSqlCall(SeverityType.RuntimeError, "Remarketing", "SGSUtility::Utilities::GetVehicleInspectionInfo", _sSql, _oParameter);
				throw e;
			}
			finally
			{
				_oParameter = null;
			}
			return functionReturnValue;
		}

		public DataSet GetYardList(int iUserID)
		{
			DataSet functionReturnValue = null;
			string _sConnString = null;
			string _sSql = null;
			SqlParameter[] _oParameter = new SqlParameter[1];

			try
			{
				_sConnString = GetConnectionString("VEHICLE");
				_sSql = "usp_REM_Get_UserYards";
				_oParameter[0] = new SqlParameter("@User_ID", iUserID);

				functionReturnValue = SqlHelper.ExecuteDataset(_sConnString, CommandType.StoredProcedure, _sSql, _oParameter);
			}
			catch (Exception e)
			{
				SaveSqlCall(SeverityType.RuntimeError, "Remarketing", "SGSUtility::Utilities::GetYardList", _sSql, _oParameter);
				throw e;
			}
			finally
			{
				_oParameter = null;
			}
			return functionReturnValue;
		}

		public DataSet GetYardList(int iUserID, int iCustId)
		{
			DataSet functionReturnValue = null;
			string _sConnString = null;
			string _sSql = null;

			try
			{
				_sConnString = GetConnectionString("VEHICLE");
				_sSql = "usp_REM_Get_UserYards " + iUserID + " , " + iCustId;

				functionReturnValue = SqlHelper.ExecuteDataset(_sConnString, CommandType.Text, _sSql);
			}
			catch (Exception e)
			{
				SaveSqlCall(SeverityType.RuntimeError, "Remarketing", "SGSUtility::Utilities::GetYardList", _sSql);
				throw e;
			}
			return functionReturnValue;
		}

		public void GetErrorEmailSettings(ref string _sEmailTo, ref string _sEmailCC, ref string _sEmailBCC, ref string _sEmailFrom)
		{
			XmlDocument _xmlDoc = null;
			XmlNodeList _oList = null;
			XmlNode _oRoot = null;

			//'_sPath = Reflection.Assembly.GetEntryAssembly.Location()
			//'_sPath = IO.Path.GetDirectoryName(_sPath) & "\"

			_xmlDoc = new XmlDocument();

			try
			{
				if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\SGSNADB.xml"))
				{
					_xmlDoc.Load(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\SGSNADB.xml");
				}
				else
				{
					_xmlDoc.Load(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\VinciService.xml");
				}

				_oRoot = _xmlDoc.GetElementsByTagName("Email").Item(0);

				foreach (XmlNode _oElem in _oRoot.ChildNodes)
				{
					switch (_oElem.Name.ToUpper())
					{
						case "TO":
							_sEmailTo = _oElem.InnerText;
							break;
						case "CC":
							_sEmailCC = _oElem.InnerText;
							break;
						case "BCC":
							_sEmailBCC = _oElem.InnerText;
							break;
						case "FROM":
							_sEmailFrom = _oElem.InnerText;
							break;
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				_oList = null;
				_xmlDoc = null;
			}
		}

		private void LogError(string _sMessage, string _sLog, string _sSource, string _sUserId = "", int _iEventId = 0)
		{
			SqlParameter[] _Parameter = new SqlParameter[6];
			string _sSql = null;
			string _sConnString = null;

			_sConnString = GetConnectionString("VRE");

			_Parameter[0] = new SqlParameter("@Event_Id", _iEventId);
			_Parameter[1] = new SqlParameter("@Error_Log", _sLog);
			_Parameter[2] = new SqlParameter("@Error_Source", _sSource);
			_Parameter[3] = new SqlParameter("@Error_Message", _sMessage);
			_Parameter[4] = new SqlParameter("@Machine_Name", Environment.MachineName);
			_Parameter[5] = new SqlParameter("@Create_User", _sUserId);

			_sSql = "USP_REM_LogError";

			try
			{
				SqlHelper.ExecuteNonQuery(_sConnString, CommandType.StoredProcedure, _sSql, _Parameter);

			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				_Parameter = null;
			}
		}

		public static object GetConsumerSettings(ref string _sConsumer, ref string _sConsumer_Section, ref string _sSeller_Section, ref string _sVehicle_Info, ref string _sTransaction_Section, ref string _sPayment_Notification, ref string _sConsumer_Header, ref string _sConsumer_Logo, ref string _sEmail_Send, ref string _sConfirmation_Email_Message,
										 ref string _sConsumer_Header_Message, ref string _sSave_Required, ref string _sRedirectURL, ref string _sInspection_Email_Message, ref string _sConfirmation_Subject, ref string _sInspection_Subject, ref string _sThankyou_Message, ref bool _bProduct, ref bool _bBackButton, ref bool _bSellerConsent,
										 ref bool _bAddVehInfo, ref string _sEmailCC, ref string _sEmailBCC, ref string _sSocketURL, ref string _sInspCountry, ref string _sExtColor, ref string _sIntColor, ref string _sMultVIN, ref string _sMakeEle, ref string _sModelEle,
										 ref string _sSeriesEle, ref string _sValidateVIN, ref string _sInspSvc, ref string _sAccept_Payment, ref string _sDisplay_Txn_Id, ref string _sListingCode)
		{
			_bAddVehInfo = true;
			object functionReturnValue = null;
			XmlNode _oRoot = null;
			XmlDocument _xmlDoc = null;
			XmlNode _oElem = null;
			string _sPath = "";

			//'_sPath = Reflection.Assembly.GetEntryAssembly.Location()
			//'_sPath = IO.Path.GetDirectoryName(_sPath) & "\"

			_xmlDoc = new XmlDocument();

			_xmlDoc.Load(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\SGSNADB.xml");

			XmlNodeList _oList = null;
			_oList = _xmlDoc.GetElementsByTagName("CONSUMER");

			_oRoot = _xmlDoc.SelectSingleNode("//Customer[@Id='" + _sConsumer + "']");

			if (_oRoot == null)
				return functionReturnValue;

			////root = xmlDoc.GetElementById("Customer")
			foreach (XmlNode _oElem_loopVariable in _oRoot.ChildNodes)
			{
				_oElem = _oElem_loopVariable;
				switch (_oElem.Name.ToUpper())
				{
					case "CONSUMER_SECTION":
						_sConsumer_Section = _oElem.InnerText;
						break;
					case "SELLER_SECTION":
						_sSeller_Section = _oElem.InnerText;
						break;
					case "VEHICLE_INFO":
						_sVehicle_Info = _oElem.InnerText;
						break;
					case "TRANSACTION_SECTION":
						_sTransaction_Section = _oElem.InnerText;
						break;
					case "PAYMENT_NOTIFICATION":
						_sPayment_Notification = _oElem.InnerText;
						break;
					case "CONSUMER_HEADER":
						_sConsumer_Header = _oElem.InnerText;
						break;
					case "CONSUMER_LOGO":
						_sConsumer_Logo = _oElem.InnerText;
						break;
					case "EMAIL_SEND":
						_sEmail_Send = _oElem.InnerText;
						break;
					case "CONFIRMATION_EMAIL_MESSAGE":
						_sConfirmation_Email_Message = _oElem.InnerText;
						break;
					case "INSPECTION_EMAIL_MESSAGE":
						_sInspection_Email_Message = _oElem.InnerText;
						break;
					case "CONSUMER_HEADER_MESSAGE":
						_sConsumer_Header_Message = _oElem.InnerText;
						break;
					case "SAVE_REQUIRED":
						_sSave_Required = _oElem.InnerText;
						break;
					case "REDIRECT_URL":
						_sRedirectURL = _oElem.InnerText;
						break;
					case "CONFIRMATION_SUBJECT":
						_sConfirmation_Subject = _oElem.InnerText;
						break;
					case "INSPECTION_SUBJECT":
						_sInspection_Subject = _oElem.InnerText;
						break;
					case "THANKYOU_MESSAGE":
						_sThankyou_Message = _oElem.InnerText;
						break;
					case "SOCKET_URL":
						_sSocketURL = _oElem.InnerText;
						break;
					case "PRODUCT_CODE":
						if (_oElem.InnerText == "Y")
						{
							_bProduct = true;
						}
						else
						{
							_bProduct = false;
						}
						break;
					case "BACK_BUTTON":
						if (_oElem.InnerText == "Y")
						{
							_bBackButton = true;
						}
						else
						{
							_bBackButton = false;
						}
						break;
					case "SELLER_CONSENT":
						if (_oElem.InnerText == "Y")
						{
							_bSellerConsent = true;
						}
						else
						{
							_bSellerConsent = false;
						}
						break;
					case "ADDITIONAL_VEHICLE_INFORMATION":
						if (_oElem.InnerText == "Y")
						{
							_bAddVehInfo = true;
						}
						else
						{
							_bAddVehInfo = false;
						}
						break;
					case "EMAIL_CC":
						_sEmailCC = _oElem.InnerText;
						break;
					case "EMAIL_BCC":
						_sEmailBCC = _oElem.InnerText;
						break;
					case "INSPECTION_COUNTRY":
						_sInspCountry = _oElem.InnerText;
						break;
					case "LISTING_CODE":
						_sListingCode = _oElem.InnerText;
						break;
					case "EXTCOLOR":
						_sExtColor = _oElem.InnerText;
						break;
					case "INTCOLOR":
						_sIntColor = _oElem.InnerText;
						break;
					case "MULTIPLE_VIN":
						_sMultVIN = _oElem.InnerText;
						break;
					case "MAKE_ELEMENT":
						_sMakeEle = _oElem.InnerText;
						break;
					case "MODEL_ELEMENT":
						_sModelEle = _oElem.InnerText;
						break;
					case "SERIES_ELEMENT":
						_sSeriesEle = _oElem.InnerText;
						break;
					case "VALIDATE_VIN":
						_sValidateVIN = _oElem.InnerText;
						break;
					case "INSPECTION_SERVICE":
						_sInspSvc = _oElem.InnerText;
						break;
					case "DISPLAY_TXN_ID":
						_sDisplay_Txn_Id = _oElem.InnerText;
						break;
					case "ACCEPT_PAYMENT":
						_sAccept_Payment = _oElem.InnerText;
						break;
				}
			}

			_oRoot = null;
			_oElem = null;
			_xmlDoc = null;
			return functionReturnValue;
		}

		public static object GetDealerRegistrationSettings(ref string _sConsumer, ref string _sDealerIdCaption, ref string _sPINCaption, ref string _sTermsConditionCaption, ref string _sVerifiedAccuracy, ref string _sRedirectURL, ref string _sPINRequired, ref string _sPaymentBypass, ref string _sConsumerLogo, ref string _sThankYouForRegistration,
										 ref string _sItemName, ref string _sRegistrationEmailFrom, ref string _sRegistrationEmailTo, ref string _sRegistrationEmailCC, ref string _sRegistrationEmailSubject, ref string _sRegistrationEmailBody)
		{
			object functionReturnValue = null;
			XmlNode _oRoot = null;
			XmlDocument _xmlDoc = null;
			XmlNode _oElem = null;
			string _sPath = "";

			//'_sPath = Reflection.Assembly.GetEntryAssembly.Location()
			//'_sPath = IO.Path.GetDirectoryName(_sPath) & "\"

			_xmlDoc = new XmlDocument();

			_xmlDoc.Load(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\SGSNADB.xml");

			XmlNodeList _oList = null;
			_oList = _xmlDoc.GetElementsByTagName("CONSUMER");

			_oRoot = _xmlDoc.SelectSingleNode("//Customer[@Id='" + _sConsumer + "']");

			if (_oRoot == null)
				return functionReturnValue;

			////root = xmlDoc.GetElementById("Customer")
			foreach (XmlNode _oElem_loopVariable in _oRoot.ChildNodes)
			{
				_oElem = _oElem_loopVariable;
				switch (_oElem.Name.ToUpper())
				{
					case "DEALER_CAPTION":
						_sDealerIdCaption = _oElem.InnerText;
						break;
					case "PIN_CAPTION":
						_sPINCaption = _oElem.InnerText;
						break;
					case "TERMS_CAPTION":
						_sTermsConditionCaption = _oElem.InnerText;
						break;
					case "ACCURACY_CAPTION":
						_sVerifiedAccuracy = _oElem.InnerText;
						break;
					case "REDIRECT_URL":
						_sRedirectURL = _oElem.InnerText;
						break;
					case "PIN_REQUIRED":
						_sPINRequired = _oElem.InnerText;
						break;
					case "PAYMENT_BYPASS":
						_sPaymentBypass = _oElem.InnerText;
						break;
					case "CONSUMER_LOGO":
						_sConsumerLogo = _oElem.InnerText;
						break;
					case "THANKYOU_REGISTRATION":
						_sThankYouForRegistration = _oElem.InnerText;
						break;
					case "ITEM_NAME":
						_sItemName = _oElem.InnerText;
						break;
					case "REGISTRATIONEMAIL_FROM":
						_sRegistrationEmailFrom = _oElem.InnerText;
						break;
					case "REGISTRATIONEMAIL_TO":
						_sRegistrationEmailTo = _oElem.InnerText;
						break;
					case "REGISTRATIONEMAIL_CC":
						_sRegistrationEmailCC = _oElem.InnerText;
						break;
					case "REGISTRATIONEMAIL_SUBJECT":
						_sRegistrationEmailSubject = _oElem.InnerText;
						break;
					case "REGISTRATIONEMAIL_BODY":
						_sRegistrationEmailBody = _oElem.InnerText;
						break;
				}
			}

			_oRoot = null;
			_oElem = null;
			_xmlDoc = null;
			return functionReturnValue;
		}

		public string getConsumerElemByTagName(string _sCustomer, string _sTag)
		{
			string functionReturnValue = null;
			XmlDocument _oDoc = new XmlDocument();
			XmlNode _oNode = default(XmlNode);

			try
			{
				_oDoc.Load(Environment.SystemDirectory + "\\sgsnadb.xml");
				_oNode = _oDoc.SelectSingleNode("//Customer[@Id='" + _sCustomer + "']/" + _sTag);
				functionReturnValue = _oNode.InnerText;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				_oDoc = null;
				_oNode = null;
			}
			return functionReturnValue;

		}

		public string getElemByTagName(string _sTag)
		{
			string functionReturnValue = null;
			XmlDocument _oDoc = new XmlDocument();
			XmlNode _oNode = default(XmlNode);

			try
			{
				_oDoc.Load(Environment.SystemDirectory + "\\sgsnadb.xml");
				_oNode = _oDoc.SelectSingleNode(_sTag);
				functionReturnValue = _oNode.InnerText;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				_oDoc = null;
				_oNode = null;
			}
			return functionReturnValue;

		}

		public void getSMTPConfig(ref string _sSendURI, ref string _sSendValue, ref string _sPickupURI, ref string _sPickupValue, ref string _sSmtpServer)
		{
			MSXML2.DOMDocument _xDoc = default(MSXML2.DOMDocument);
			string XMLSource = null;
			MSXML2.IXMLDOMNode _Root = default(MSXML2.IXMLDOMNode);
			MSXML2.IXMLDOMNode _Child = default(MSXML2.IXMLDOMNode);
			int _Indx = 0;

			try
			{
				_xDoc = new MSXML2.DOMDocument();
				XMLSource = "SGSNADB.xml";
				_xDoc.load(XMLSource);
				_Root = _xDoc.getElementsByTagName("SMTP")[0];
				for (_Indx = 0; _Indx <= _Root.childNodes.length - 1; _Indx++)
				{
					_Child = _Root.childNodes[_Indx];
					switch (_Child.nodeName.ToUpper())
					{
						case "SENDUSING_URI":
							_sSendURI = _Child.text;
							break;
						case "SENDUSING_VALUE":
							_sSendValue = _Child.text;
							break;
						case "PICKUPFOLDER_URI":
							_sPickupURI = _Child.text;
							break;
						case "PICKUPFOLDER_VALUE":
							_sPickupValue = _Child.text;
							break;
						case "SMTPSERVER":
							_sSmtpServer = _Child.text;
							break;
					}
				}
			}
			catch (Exception ex)
			{
				//WriteErrorLog("SGSUtility:Utilities", "getSMTPConfig", ex.Message, "")
			}
			finally
			{
				if (string.IsNullOrEmpty(_sSendURI.Trim()))
				{
					_sSendURI = "http://schemas.microsoft.com/cdo/configuration/senduing";
				}

				if (string.IsNullOrEmpty(_sSendValue.Trim()))
				{
					_sSendValue = "1";
				}

				if (string.IsNullOrEmpty(_sPickupURI.Trim()))
				{
					_sPickupURI = "http://schemas.microsoft.com/cdo/configuration/smtpserverpickupdirectory";
				}

				if (string.IsNullOrEmpty(_sPickupValue.Trim()))
				{
					_sPickupValue = "c:\\inetpub\\mailroot\\pickup";
				}

				_xDoc = null;
				_Root = null;
				_Child = null;
			}
		}

		public void SaveSqlCall(SeverityType _severityType, string _lineOfBusiness, string _sModule, string _sSql, SqlParameter[] _oParameters = null)
		{
			string _sParamString = "";

			if ((_oParameters != null))
			{
				for (int i = 0; i < _oParameters.Length; i++)
				{
					//if the parameter value is NULL
					if (DBNull.Value == _oParameters[i].Value)
					{
						_sParamString += _oParameters[i].ParameterName + "= " + "NULL, " + "\r\n";
					}
					else
					{
						if (_oParameters[i].DbType == DbType.String | _oParameters[i].DbType == DbType.StringFixedLength | _oParameters[i].DbType == DbType.DateTime | _oParameters[i].DbType == DbType.AnsiString | _oParameters[i].DbType == DbType.AnsiStringFixedLength)
						{
							_sParamString += _oParameters[i].ParameterName + "= " + "'" + Convert.ToString(_oParameters[i].Value) + "', " + "\r\n";
						}
						else
						{
							_sParamString += _oParameters[i].ParameterName + "= " + Convert.ToString(_oParameters[i].Value) + ", " + "\r\n";
						}
					}
				}

				if (!string.IsNullOrEmpty(_sParamString))
				{

					_sParamString = _sParamString.Substring(0, (_sParamString.Length) - 2).Trim();
				}
			}

			if (_sParamString.EndsWith(","))
			{
				_sParamString = _sParamString.Substring(0, (_sParamString.Length) - 1).Trim();
			}

			ErrorInfo.sqlCall = _sSql + " " + _sParamString;
			ErrorInfo.moduleName = _sModule;
			ErrorInfo.severityType = _severityType;
			ErrorInfo.lineOfBusiness = _lineOfBusiness;

			HttpContext.Current.Session["ErrInfo"] = ErrorInfo;
			//oContext.Session("SQL_Call") = _sSql + " " + _sParamString
		}

		public void WriteErrorLog(string _sClassName, string _sFunctionName, string _sExceptionMessage, string _sSql, SqlParameter[] _oParameters = null, int _iEventId = 0, string _sUserId = "", string _sLog = "ACV Error Manager", string _sSource = "ACV Web", bool _bSendMail = true,
										string _sTo = "autonam.alert.app@acvauctions.com", string _sFrom = "ACV Web", string _sCC = "aseidel@acvauctions.com")
		{
			System.Diagnostics.EventLog _evtEm = null;

			string _sMessage = null;
			string _sParamString = null;
			int i = 0;

			GetErrorEmailSettings(ref _sEmailTo, ref _sEmailCC, ref _sEmailBCC, ref _sEmailFrom);
			try
			{
				_evtEm = new System.Diagnostics.EventLog(_sLog);

				if (!System.Diagnostics.EventLog.SourceExists(_sSource))
				{
					System.Diagnostics.EventLog.CreateEventSource(_sSource, _sLog);
				}

				_evtEm.Source = _sSource;

				//build Parameter string from SqlParameter object
				if ((_oParameters != null))
				{
					_sParamString = "";
					for (i = 0; i <= _oParameters.Length; i++)
					{
						//if the parameter value is NULL
						if (_oParameters[i].Value == DBNull.Value)
						{
							_sParamString += _oParameters[i].ParameterName + "= " + "NULL, " + "\r\n";
						}
						else
						{
							if (_oParameters[i].DbType == DbType.String | _oParameters[i].DbType == DbType.StringFixedLength | _oParameters[i].DbType == DbType.DateTime | _oParameters[i].DbType == DbType.AnsiString | _oParameters[i].DbType == DbType.AnsiStringFixedLength)
							{
								_sParamString += _oParameters[i].ParameterName + "= " + "'" + Convert.ToString(_oParameters[i].Value) + "', " + "\r\n";
							}
							else
							{
								_sParamString += _oParameters[i].ParameterName + "= " + Convert.ToString(_oParameters[i].Value) + ", " + "\r\n";
							}
						}
					}

					if (!string.IsNullOrEmpty(_sParamString))
					{
						//_sParamString = Strings.Left(_sParamString, Strings.Len(_sParamString) - 2);
						_sParamString = _sParamString.Substring(0, (_sParamString.Length) - 2);
					}
				}

				//Build Message String
				if (_iEventId == 0)
				{
					_sMessage = "<b>Function</b> ->  " + _sClassName + " <b>:</b> " + _sFunctionName + " " + "\r\n" + "<b>Error :</b> " + _sExceptionMessage + " " + "\r\n" + "<b>Procedure Call :</b> " + _sSql + " " + "\r\n" + _sParamString;
				}
				else
				{
					_sMessage = "<b>Error :</b> " + _sExceptionMessage;
				}

				try
				{
					//Log error in database
					LogError(_sMessage, _sLog, _sSource, _sUserId, _iEventId);
				}
				catch (Exception e)
				{
					//If error could not be logged in database, write in event log
					_evtEm.WriteEntry(_sMessage, EventLogEntryType.Error);
				}

				string _sSubject = null;

				_sSubject = "Web Server Error in " + _sClassName + ":" + _sFunctionName;

				if (_bSendMail == true)
				{
					if (_sFrom == "ACV Web")
					{
						_sFrom = "asi.services@acvauctions.com";
					}
					SendEmail(_sEmailTo, _sFrom, _sSubject, _sMessage, _sEmailCC, _sEmailBCC);
				}
			}
			catch (Exception ex)
			{
				//do nothing
				_evtEm.WriteEntry(ex.ToString(), EventLogEntryType.Error);

			}
			finally
			{
				_evtEm = null;
			}
		}

	}
}
