using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
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
using System.Xml.Serialization;

namespace BroforceBfg2Bfc
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

		public static CampaignHeader bfgCampaignHeader;
		public static byte[] bfgCampaignData;
		public static string bfgCampaignFileName;

		public static CampaignHeader GetCampaignHeader(byte[] buffer)
        {
			int xmlEndIndex = -1;
			for (int i = 0; i < buffer.Length; i++)
			{
				if (buffer[i] == 31)
				{
					xmlEndIndex = i;
					break;
				}
			}
			if (xmlEndIndex < -1)
			{
				throw new IOException("Campaign appears to be in the incorrect format.");
			}
			CampaignHeader campaignHeader;
			using (MemoryStream memoryStream2 = new MemoryStream(buffer, 0, xmlEndIndex))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(CampaignHeader));
				campaignHeader = (CampaignHeader)xmlSerializer.Deserialize(memoryStream2);
			}
			return campaignHeader;
		}

		public static byte[] All2Xml(byte[] buffer)
		{
			int xmlEndIndex = -1;
			for (int i = 0; i < buffer.Length; i++)
			{
				if (buffer[i] == 31)
				{
					xmlEndIndex = i;
					break;
				}
			}
			if (xmlEndIndex < -1)
			{
				throw new IOException("Campaign appears to be in the incorrect format.");
			}
			CampaignHeader campaignHeader;
			using (MemoryStream memoryStream2 = new MemoryStream(buffer, 0, xmlEndIndex))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(CampaignHeader));
				campaignHeader = (CampaignHeader)xmlSerializer.Deserialize(memoryStream2);
			}
			bool bfg = false;
			if (bfgCampaignFileName.EndsWith(".bfg"))
			{
				xmlEndIndex++;
				bfg = true;
			}
			int dataSize = buffer.Length - xmlEndIndex;
			byte[] array = new byte[dataSize];
			Buffer.BlockCopy(buffer, xmlEndIndex, array, 0, dataSize);

            if (bfg)
            {
				array = DecryptBlob(array, campaignHeader.md5);
			}
			array = CLZF2.Decompress(array);

			//Array.Clear(array, xmlEndIndex-1, 1);
            byte[] header = new byte[bfg ? xmlEndIndex - 1 : xmlEndIndex];
            Buffer.BlockCopy(buffer, 0, header, 0, bfg ? xmlEndIndex - 1 : xmlEndIndex);
            //byte[] body = new byte[array.Length - xmlEndIndex - 1];
            //Buffer.BlockCopy(array, xmlEndIndex + 1, body, 0, array.Length - xmlEndIndex - 1);
            byte[] result = new byte[array.Length + header.Length];
            Buffer.BlockCopy(header, 0, result, 0, header.Length);
            Buffer.BlockCopy(array, 0, result, header.Length, result.Length - header.Length);
            return result;
		}

		public static byte[] Bfg2Bfc(byte[] buffer)
		{
			int xmlEndIndex = -1;
			for (int i = 0; i < buffer.Length; i++)
			{
				if (buffer[i] == 31)
				{
					xmlEndIndex = i;
					break;
				}
			}
			if (xmlEndIndex < -1)
			{
				throw new IOException("Campaign appears to be in the incorrect format.");
			}
			CampaignHeader campaignHeader;
			using (MemoryStream memoryStream2 = new MemoryStream(buffer, 0, xmlEndIndex))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(CampaignHeader));
				campaignHeader = (CampaignHeader)xmlSerializer.Deserialize(memoryStream2);
			}
			bool bfg = false;
			if (bfgCampaignFileName.EndsWith(".bfg"))
			{
				xmlEndIndex++;
				bfg = true;
			}
			int dataSize = buffer.Length - xmlEndIndex;
			byte[] array = new byte[dataSize];
			Buffer.BlockCopy(buffer, xmlEndIndex, array, 0, dataSize);

			if (bfg)
			{
				array = DecryptBlob(array, campaignHeader.md5);
			}
			//array = CLZF2.Decompress(array);

			//Array.Clear(array, xmlEndIndex-1, 1);
			byte[] header = new byte[bfg ? xmlEndIndex - 1 : xmlEndIndex];
			Buffer.BlockCopy(buffer, 0, header, 0, bfg ? xmlEndIndex - 1 : xmlEndIndex);
			//byte[] body = new byte[array.Length - xmlEndIndex - 1];
			//Buffer.BlockCopy(array, xmlEndIndex + 1, body, 0, array.Length - xmlEndIndex - 1);
			byte[] result = new byte[array.Length + header.Length];
			Buffer.BlockCopy(header, 0, result, 0, header.Length);
			Buffer.BlockCopy(array, 0, result, header.Length, result.Length - header.Length);
			return result;
		}

		private static byte[] DecryptBlob(byte[] blob, string key)
		{
			DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider
			{
				Key = Encoding.ASCII.GetBytes(key.Substring(0, 8)),
				IV = new byte[]
				{
				5,
				2,
				177,
				3,
				1,
				28,
				13,
				69
				}
			};
			ICryptoTransform cryptoTransform = descryptoServiceProvider.CreateDecryptor();
			return cryptoTransform.TransformFinalBlock(blob, 0, blob.Length);
		}

        private void Grid_Drop(object sender, DragEventArgs e)
        {
			var fileName = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
			bfgCampaignData = File.ReadAllBytes(fileName);
			bfgCampaignHeader = GetCampaignHeader(bfgCampaignData);
			bfgCampaignFileName = fileName;
            if (bfgCampaignFileName.EndsWith(".bfg"))
            {
				ButtonBfg2Bfc.IsEnabled = true;
			}
			ButtonAll2Xml.IsEnabled = true;
			

			TextBlockName.Text = bfgCampaignHeader.name;
			TextBlockAuthor.Text = bfgCampaignHeader.author;
			TextBlockDescription.Text = bfgCampaignHeader.description;
			TextBlockMD5.Text = bfgCampaignHeader.md5;
		}

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.Link;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
		}

        private void ButtonBfg2Bfc_Click(object sender, RoutedEventArgs e)
        {
			byte[] bfcCampaignData = Bfg2Bfc(bfgCampaignData);
			File.WriteAllBytes($"{bfgCampaignFileName.Replace(".bfg","_bfcFile")}.bfc", bfcCampaignData);
			MessageBox.Show("成功转为.bfc文件", "成功", MessageBoxButton.OK, MessageBoxImage.Information);

			ButtonBfg2Bfc.IsEnabled = false;
			ButtonAll2Xml.IsEnabled = false;
        }

        private void ButtonAll2Xml_Click(object sender, RoutedEventArgs e)
        {
			byte[] bfcCampaignData = All2Xml(bfgCampaignData);
			File.WriteAllBytes($"{bfgCampaignFileName.Replace(bfgCampaignFileName.EndsWith(".bfg") ? ".bfg" : ".bfc", "_xmlFile")}.xml", bfcCampaignData);
			MessageBox.Show("成功转为.xml文件", "成功", MessageBoxButton.OK, MessageBoxImage.Information);

			ButtonAll2Xml.IsEnabled = false;
		}
    }
}
