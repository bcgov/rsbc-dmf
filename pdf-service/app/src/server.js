"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = __importDefault(require("express"));
const multer_1 = __importDefault(require("multer"));
const path_1 = __importDefault(require("path"));
const fs_1 = __importDefault(require("fs"));
const app = (0, express_1.default)();
const port = 3000;
// Set up multer storage
const storage = multer_1.default.diskStorage({
    destination: 'uploads/',
    filename: (_, file, cb) => {
        cb(null, Date.now() + '-' + file.originalname);
    },
});
const upload = (0, multer_1.default)({ storage });
// Serve static HTML pages
app.use(express_1.default.static(path_1.default.join(__dirname, 'views')));
// Form page
app.get('/', (_req, res) => {
    res.sendFile(path_1.default.join(__dirname, 'views', 'form.html'));
});
// Handle form submission
app.post('/upload', upload.fields([{ name: 'file1' }, { name: 'file2' }]), (req, res) => {
    const files = req.files;
    const file1Name = files?.file1?.[0]?.originalname || 'No file';
    const file2Name = files?.file2?.[0]?.originalname || 'No file';
    const resultHtml = fs_1.default.readFileSync(path_1.default.join(__dirname, 'views', 'result.html'), 'utf8');
    const rendered = resultHtml
        .replace('{{file1}}', file1Name)
        .replace('{{file2}}', file2Name);
    res.send(rendered);
});
app.listen(port, () => {
    console.log(`Server running at http://localhost:${port}`);
});
