import fs from "node:fs";
import path from "node:path";

const projectRoot = process.cwd();
const srcDir = path.resolve(projectRoot, "src");

// Chỉ bắt import bắt đầu bằng "../" (đi ra ngoài thư mục hiện tại) -
// import cùng cấp kiểu "./Something" giữ nguyên, không cần alias hóa.
const importRegex = /(from\s+["'])(\.\.\/[^"']+)(["'])/g;

function walk(dir, callback) {
  for (const entry of fs.readdirSync(dir, { withFileTypes: true })) {
    const fullPath = path.join(dir, entry.name);
    if (entry.isDirectory()) {
      if (entry.name === "node_modules") continue;
      walk(fullPath, callback);
    } else if (/\.(js|jsx|ts|tsx)$/.test(entry.name)) {
      callback(fullPath);
    }
  }
}

let changedFiles = 0;

walk(srcDir, (filePath) => {
  const content = fs.readFileSync(filePath, "utf8");
  const fileDir = path.dirname(filePath);

  const newContent = content.replace(
    importRegex,
    (match, prefix, importPath, suffix) => {
      const absoluteImportPath = path.resolve(fileDir, importPath);
      const relativeToSrc = path.relative(srcDir, absoluteImportPath);

      // Nếu vì lý do gì đó import trỏ ra ngoài cả src thì bỏ qua, giữ nguyên.
      if (relativeToSrc.startsWith("..")) return match;

      const aliasPath = "@/" + relativeToSrc.split(path.sep).join("/");
      return `${prefix}${aliasPath}${suffix}`;
    },
  );

  if (newContent !== content) {
    fs.writeFileSync(filePath, newContent, "utf8");
    changedFiles++;
    console.log("Đã sửa:", path.relative(projectRoot, filePath));
  }
});

console.log(`\nHoàn tất. Đã sửa ${changedFiles} file.`);
