Browshot version 1.10.0
================================


Browshot (http://www.browshot.com/) is a web service to easily make screenshots of web pages in any screen size, as any device: iPhone, iPad, Android, Nook, PC, etc. Browshot has full Flash, JavaScript, CSS, & HTML5 support.


The latest API version is detailed at https://browshot.com/api/documentation. Browshot follows the API documentation very closely: the function names are similar to the URLs used (screenshot/create becomes ScreenshotCreate(), instance/list becomes IinstanceList(), etc.), the request arguments are exactly the same, etc.


The library version matches closely the API version it handles: browshot 1.0.0 is the first release for the API 1.0, browshot 1.1.1 is the second release for the API 1.1, etc.


Browshot can handle most the API updates within the same major version, e.g. browshot 1.0.0 should be compatible with the API 1.1 or 1.2.


The source code can be found on github at https://github.com/juliensobrier/browshot-csharp


== NOTES ==

This is the very first release of the C# library. Documentation is missing, take a look at the unit tests to understand how to use the library.

The API calls return Dictionary<string, Object>. This will likely change to stronger types in future releases.
