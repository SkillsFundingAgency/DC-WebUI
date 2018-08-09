/// <reference path="node_modules/gulp-sass/index.js" />
/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
    sass = require("gulp-sass"),
    rename = require("gulp-rename"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin");

var paths = {
    webroot: "wwwroot/"
};

paths.css = paths.webroot + "assets/stylesheets";
paths.cssBuild = paths.webroot + "build";

var sourcemaps = require('gulp-sourcemaps');

gulp.task('sass', function () {
    return gulp.src('wwwroot/scss/**/*.scss')
        .pipe(sourcemaps.init())
        .pipe(sass({
            includePaths: ['node_modules/govuk_frontend/settings'
            ]
        }).on('error', sass.logError))
        .pipe(sourcemaps.write())
        .pipe(gulp.dest(paths.css))
        .pipe(concat('site.css'))
        .pipe(cssmin())
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(paths.cssBuild));
});

gulp.task('sass:watch', function () {
    gulp.watch('wwwroot/scss/**/*.scss', ['sass']);
});
