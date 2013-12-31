SignSafariAPNPackages
=====================

Create manifest and signature files for Safari push notification push package

You can run this application from the command line. 

You need to have already have created the images and JSON files in the required format:

icon.iconset/icon_16x16.png <br />
icon.iconset/icon_16x16@2x.png <br />
icon.iconset/icon_32x32.png <br />
icon.iconset/icon_32x32@2x.png <br />
icon.iconset/icon_128x128.png <br />
icon.iconset/icon_128x128@2x.png<br />
website.json<br />

See more <a href='https://developer.apple.com/library/mac/documentation/NetworkingInternet/Conceptual/NotificationProgrammingGuideForWebsites/PushNotifications/PushNotifications.html'>here.</a>

Pass in the following arguments to reconcile where the package is located, where the APN cert is located and the certs password:

Path
CertPath
CertName
Password

Here is an example of how this would work:

"SignSafariAPNPackages.exe" path=C:\Users\test\Documents\SafariTest\Test.pushpackage certpath=C:\Users\test\Documents\SafariTest certname=WebsitePushCert.p12 password=testpassword
