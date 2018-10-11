using System;
using System.Windows.Forms;

namespace Lab3
{
	public class IKEAMyntAtare2000
	{
		public void starta()
		{
			MessageBox.Show ("Welcome to the NS ticket vending machine.");
		}

		public void stoppa()
		{
			MessageBox.Show ("Thank you!");
		}

		public void betala(float pris)
		{
			MessageBox.Show (pris + " euro");
		}
	}
}

