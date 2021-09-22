# What is ArchiveFromZoomCloud?
Utility that leverages the Zoom API to comb through every cloud recording, save its META data into an assigned database and then FTP those files to a dedicated location for archiving. Once the utility marks the files as archived, it once again leverages the Zoom API to remove it from the Zoom cloud to free up the cloud storage. 
# Before Getting Started
You need to have create a Zoom API App that uses JWT Credentials.
For instructions on how to create and obtain your Zoom API app, visit https://marketplace.zoom.us/docs/guides/build
