# APIInfo
Provides some of the functionality that the Steeltoe Management Health library did.  I was having issues with that library consistently running as well as difficulty with some of the documentation.  I also wanted some other features.  

## Simplified Use
You just need to make a couple of additions to your hostbuilded to add an object and then a couple of entries to startup.cs to add them to the pipeline.

```<language>
public static IHostBuilder CreateHostBuilder(string[] args) =>
	Host.CreateDefaultBuilder(args)
		.ConfigureServices((hostContext, services) => {
			// Set APIInfo Object and override the default root path to infotest...
			APIInfoBase apiInfoBase = new ("infotest");
			apiInfoBase.AddConfigMatchCriteria("password",false,true);
			apiInfoBase.AddConfigMatchCriteria("os", false, true);
			apiInfoBase.AddConfigMatchCriteria("LogLevel", true, true);
			apiInfoBase.AddConfigMatchCriteria("environment", false, false);

			services.AddSingleton<IAPIInfoBase>(apiInfoBase);

			// Add a SimpleInfo retriever - Host Information
			services.AddTransient<ISimpleInfoRetriever, SimpleRetrieverHostInfo>();
```

The APIInfoBase object is required as is the services.AddSingleton<IAPIInfoBase>(apiInfoBase) line.  The AddConfigMatchCriteria is an optional piece discussed later.


The startup.cs file needs the following additions:
```<language>
app.UseEndpoints(endpoints =>
{		
	endpoints.MapControllers();
	endpoints.MapSlugEntPing();
	endpoints.MapSlugEntSimpleInfo();
	endpoints.MapSlugEntConfig();
});
```

The endpoints.MapSlug... lines just activate various endpoint info.

### MapSlugEntPing
Provides a very simple response in the form of ping...pong.  Just tells you the service is responding and working.  Can use it for automations that just need to know is the service is responding or not.

### MapSlugEntSimpleInfo 
Provides some basic information about the service that you can customize.  The simple info page can be divided into sections with each section getting customized information from a class you write.  

### MapSlugEntConfig
Provides configuration (IConfiguration) related information about the API service that it has while running.  You have to be careful with this module as it will display your entire Configuration object.
It therefore provides the ability to match on entire Key names or partial key names to hide values that you do not wish displayed.  Values that are marked hidden are never sent to the web page, but instead are replaced with the word HIDDEN.  They therefore cannot be leaked by viewing the page source.

## Sample App
There is a sample app that demonstrates the key components.
