
using MediaDevices;

var devices = MediaDevice.GetDevices();

//safe copy only non-already downloaded files (speed up)
List<string> filesalread = Directory.GetFiles(@"D:\foto came\2022-07-15 photos xiaomi fanis\Camera").
        Select(f => Path.GetFileName(f)).ToList();

using (var device = devices.First(d => d.FriendlyName == "Mi Note 10"))
{
    device.Connect();
    var photoDir = device.GetDirectoryInfo(@"\Internal shared storage\DCIM\Camera");
    //var photoDir = device.GetDirectoryInfo(@"\Internal shared storage\Download");

    var files = photoDir.EnumerateFiles("*.*", SearchOption.AllDirectories);

    foreach (var file in files)
    {
        if (!filesalread.Contains(file.Name))
        {
            Console.WriteLine($"{file.Name} is being written ....");

            MemoryStream memoryStream = new System.IO.MemoryStream();
            device.DownloadFile(file.FullName, memoryStream);
            memoryStream.Position = 0;
            WriteStreamToDisk($@"D:\foto came\2022-07-15 photos xiaomi fanis\Camera\{file.Name}", memoryStream);
        }
        else Console.WriteLine($"{file.Name} already on disk");
    }
    device.Disconnect();
}

void WriteStreamToDisk(string filePath, MemoryStream memoryStream)
{
    using (FileStream file = new FileStream(filePath, FileMode.Create, System.IO.FileAccess.Write))
    {
        byte[] bytes = new byte[memoryStream.Length];
        memoryStream.Read(bytes, 0, (int)memoryStream.Length);
        file.Write(bytes, 0, bytes.Length);
        memoryStream.Close();
    }
}
