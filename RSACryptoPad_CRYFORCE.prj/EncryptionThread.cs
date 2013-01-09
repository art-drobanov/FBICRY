using System;
using System.Windows.Forms;

using CRYFORCE.Engine;

namespace RSACryptoPad
{
	public class EncryptionThread
	{
		private ContainerControl containerControl;
		private Delegate finishedProcessDelegate;
		private Delegate updateTextDelegate;

		public void Encrypt(object inputObject)
		{
			var inputObjects = (object[])inputObject;
			containerControl = (Form)inputObjects[0];
			finishedProcessDelegate = (Delegate)inputObjects[1];
			updateTextDelegate = (Delegate)inputObjects[2];
			string encryptedString = RSA.EncryptString((string)inputObjects[3], (string)inputObjects[4]);
			containerControl.Invoke(updateTextDelegate, new object[] {encryptedString});
			containerControl.Invoke(finishedProcessDelegate);
		}

		public void Decrypt(object inputObject)
		{
			var inputObjects = (object[])inputObject;
			containerControl = (Form)inputObjects[0];
			finishedProcessDelegate = (Delegate)inputObjects[1];
			updateTextDelegate = (Delegate)inputObjects[2];
			string decryptedString = RSA.DecryptString((string)inputObjects[3], (string)inputObjects[4]);
			containerControl.Invoke(updateTextDelegate, new object[] {decryptedString});
			containerControl.Invoke(finishedProcessDelegate);
		}
	}
}