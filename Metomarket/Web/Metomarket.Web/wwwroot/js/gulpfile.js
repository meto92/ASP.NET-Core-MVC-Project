"use strict";

const gulp = require("gulp");
const autoprefixer = require("gulp-autoprefixer");
const browserSync = require('browser-sync').create();
const sass = require('gulp-sass');

const port = 44344;
const proxyAddress = `https://localhost:${port}`;
const scssFiles = "../scss/**/*.scss";
const cssDirectory = "../css/";
const jsFiles = "./**/*.js";
const cshtmlFiles = "../../**/*.cshtml";

sass.compiler = require('node-sass');

browserSync.init({
    proxy: proxyAddress
});

gulp.task("scss", () => {
    return gulp.src(scssFiles)
        .pipe(sass(/*{outputStyle: "compressed"}*/).on('error', sass.logError))
        .pipe(autoprefixer({
            browserlist: ["last 2 versions"],
            cascade: false
        }))
        .pipe(gulp.dest(cssDirectory))
        .pipe(browserSync.stream({match: "**/*.css"}));
});

gulp.task("watch", () => {
    gulp.watch(scssFiles, gulp.series("scss"));

    gulp.watch([
        jsFiles,
        cshtmlFiles
    ]).on("change", browserSync.reload);
});

gulp.task("default", gulp.series("scss", "watch"));