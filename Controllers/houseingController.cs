using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace codetestapi.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class houseingController : ControllerBase
  {
    private readonly ILogger<houseingController> logger;
    private readonly settings settings;

    public houseingController(
                  ILogger<houseingController> logger,
                  IOptions<settings> settings)
    {
      this.logger = logger;
      this.settings = settings.Value;
    }

    [HttpGet]
    public IActionResult Get(string update)
    {
      
      try
      {
        if (update == "t")
        {        

          var json = BuildJsonFromXml();

          BuildJsonTextFile(json);

          PurgeOldJson();
        }
      }
      catch (FileAllreadyExistsException ex)
      {

        logger.LogError($@"houseing/get; {ex.Message}");
      }

      return Content(
                  GetJsonFromFile(
                      getlastCashedJsonFile()
                    )
                );
    }

    private string GetXmlFromSource(){
      
      using (var wc = new WebClient())
      using (var stream = wc.OpenRead(settings.source_url))
      using (var reader = new StreamReader(stream))
      {

        var output = reader.ReadToEnd();
        return output.TrimStart();
      }
    
    }

    private XmlNode BuildXmlObject()
    {
      logger.LogDebug($@"Getting xml from source url");
            
      XmlDocument doc = new XmlDocument();

      var s = GetXmlFromSource();

      doc.LoadXml(s);

      var r = doc.GetElementsByTagName("lbstream");
      var x = r.Item(0).ChildNodes.Item(0);

      return x;
    }

    private string BuildJsonFromXml()
    {
      logger.LogDebug("Serialize json from xml");
      
      var doc = BuildXmlObject();
      
      return JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);
    }

    private void BuildJsonTextFile(string json)
    {
      logger.LogDebug("Building json textfile");

      var fullpath = BuildFullPath();

      if (System.IO.File.Exists(fullpath) == true)
      {

        throw new FileAllreadyExistsException();
      }

      using (StreamWriter outputFile = new StreamWriter(fullpath))
      {
        outputFile.WriteLine(json);
      }
    }

    private string BuildFullPath()
    {

      logger.LogDebug("Building json filename.");
      
      var year = DateTime.Now.Year;
      var month = DateTime.Now.Month;
      var day = DateTime.Now.Day;
      var hour = DateTime.Now.Hour;
      var min = DateTime.Now.Minute;

      var currentdirectory = WorkingDirectory;
      var filename = $@"parsel_{year}-{month}-{day}_{hour}-{min}.json";
      var fullpath = Path.Combine(currentdirectory, filename);

      return fullpath;
    }

    private string WorkingDirectory
    {
      get
      {
        
        return Directory.GetCurrentDirectory();
      }
    }

    private FileInfo getlastCashedJsonFile()
    {
      logger.LogDebug("Getting last json file created");
    
      var datafile = GetParselJsonFiles();

      if (datafile.Count() == 0)
      {
        throw new FileNotFoundException("No json files found.");
      }

      return datafile.First();
    }

    private string GetJsonFromFile(FileInfo file)
    {
      logger.LogDebug("Getting json data from file.");

      using (var fs = file.OpenRead())
      using (var s = new StreamReader(fs))
      {
 
        return s.ReadToEnd();
      }
    }

    private void PurgeOldJson(){
      logger.LogDebug("Purgeing old json files.");

      var files = GetParselJsonFiles();

      var x = 0;
      
      foreach(var file in files){
          if (x >= 5){
            file.Delete();
          }

          x += 1;
      }  
     }

    private FileInfo[] GetParselJsonFiles(){
      logger.LogDebug("Get list of json datafiles.");

      var files = new DirectoryInfo(WorkingDirectory)
                    .GetFiles("parsel*")
                    .OrderByDescending(e => e.Name);

      return files.ToArray();
    }
  }
}
