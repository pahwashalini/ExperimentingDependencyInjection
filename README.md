# ExperimentingDependencyInjection
resolving dependency injection  of interface with multiple implementations

I have 3 Logger implementation of ILogs Interface 
1. Text
2. CSV
3. Sql Server

#First way

In ConfigureServices Method of startup class

        services.AddTransient<Func<string, ILogs>>((provider) =>
            {
            return new Func<string, ILogs>(
                (logtype) => new FactoryLogger().GetLogger(logtype)                                            
                );
            });
 property in startup.cs
            public string logtype { get; set; }
In Values Controller

        ILogs _log;
        public ValuesController(Func<string, ILogs> logFactory)
        {
            _log = logFactory("text");
        }
        [HttpPost]
        public void Post([FromBody] string v)
        {          
            _log.Writelog("at last its done");            
        }
Factory class

        public class FactoryLogger
        {    
        public ILogs GetLogger(string LoggerType)
        {

            if (LoggerType == "text")
            {
                return new LogWriterText();
            }
            else if (LoggerType == "csv")
            {
                return new LogWriterSql();
            }
            else if (LoggerType == "sql")
            {
                return new LogWriterSql();
            }
            return null;      
           
        }
    }
#second way

In ConfigureServices Method in Startup.cs

            services.AddTransient<ILogs, LogWriterSql>();
            services.AddTransient<ILogs, LogWriterCSV>();
            services.AddTransient<ILogs, LogWriterText>();
            services.AddTransient<Func<LoggerType, ILogs>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case LoggerType.csv:
                        return serviceProvider.GetService<LogWriterCSV>();
                    case LoggerType.sql:
                        return serviceProvider.GetService<LogWriterSql>();
                    case LoggerType.text:
                        return serviceProvider.GetService<LogWriterText>();
                    default:
                        throw new KeyNotFoundException();
                }
            });
            
            //Enum in Startup.cs
                  public enum LoggerType
                  {  text = 0,
                      sql = 1,
                      csv = 2
                  }
              
  In Values Controller
       
         private ILogs _log;
              public ValuesController(ILogs log)
              {
                  _log = log;
              }
         [HttpPost]
        public void Post([FromBody] string logtype)
        {
            LoggerType loggertype = (LoggerType)Enum.Parse(typeof(LoggerType), logtype);            
            _log.Writelog("at last its done");            
        }
