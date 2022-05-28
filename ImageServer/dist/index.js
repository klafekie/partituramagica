"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = __importDefault(require("express"));
const serve_index_1 = __importDefault(require("serve-index"));
const app = (0, express_1.default)();
const port = 8080; // default port to listen
// app.use(serveStatic('public/ftp', { index: ['default.html', 'default.htm'] }))
const partituraRoot = 'c:\\Users\\falke\\OneDrive - adesso Group\\My Partitura Files\\';
app.use('/open', (0, serve_index_1.default)(partituraRoot, {
    filter: (fileName) => fileName.endsWith('.partitura2') || !fileName.includes('.')
}));
app.use('/partitura', express_1.default.static(partituraRoot));
// define a route handler for the default home page
app.get("/image/:path/:x.:y", (req, res) => {
    res.send(req.params);
});
// start the Express server
app.listen(port, () => {
    // console.log(`server started at http://localhost:${port}`);
});
//# sourceMappingURL=index.js.map