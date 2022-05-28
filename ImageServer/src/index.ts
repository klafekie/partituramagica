import express from "express";
import path from "path";
import serveStatic from 'serve-static';
import serveIndex from 'serve-index';
const app = express();
const port = 8080; // default port to listen

// app.use(serveStatic('public/ftp', { index: ['default.html', 'default.htm'] }))
const partituraRoot = 'c:\\Users\\falke\\OneDrive - adesso Group\\My Partitura Files\\';
app.use('/open', serveIndex(partituraRoot, {
	filter: (fileName:string) => fileName.endsWith('.partitura2') || !fileName.includes('.')
}));
app.use('/partitura', express.static(partituraRoot));
// define a route handler for the default home page
app.get("/image/:path/:x.:y", (req, res) => {
	  res.send(req.params)
});

// start the Express server
app.listen(port, () => {
	// console.log(`server started at http://localhost:${port}`);
});
