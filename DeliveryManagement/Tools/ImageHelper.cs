using System.Drawing;
using System.IO;

namespace DeliveryManagement.Tools
{
	public static class ImageHelper
	{
		private const string SignaturesDirectory = ".\\Signatures";

		public static void SaveImage(string fileName, Image image)
		{
			var imagePath = Path.Combine(Directory.GetCurrentDirectory(), SignaturesDirectory);

			if (!Directory.Exists(imagePath))
				Directory.CreateDirectory(imagePath);

			var filePath = Path.Combine(imagePath, fileName);

			if (File.Exists(filePath))
				File.Delete(filePath);

			image.Save(filePath);
		}
	}
}
