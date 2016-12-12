'use strict';

const Bower = require('gulp-bower');
const Gulp = require('gulp');
const Rename = require('gulp-rename');
const Sass = require('gulp-sass');

const config = {
    bowerDir: "./bower_components",
    fontDest: "./wwwroot/fonts",
    jsSrc: "./Assets/Javascript/*",
    jsDest: "./wwwroot/js",
    sassSrc: "./Assets/Sass/Site.scss",
    sassDest: "./wwwroot/css"
};

Gulp.task('default', ['bower', 'icons', 'javascript', 'sass']);

Gulp.task('bower', function () {
    return Bower()
        .pipe(Gulp.dest(config.bowerDir));
});

Gulp.task('icons', ['bower'], function () {
    return Gulp.src(config.bowerDir + '/fontawesome/fonts/**.*')
        .pipe(Gulp.dest(config.fontDest));
});

Gulp.task('javascript', ['javascriptSource', 'javascriptVendor']);

Gulp.task('javascriptSource', function () {
    return Gulp.src(config.jsSrc)
        .pipe(Gulp.dest(config.jsDest));
});

Gulp.task('javascriptVendor', ['bower'], function () {
    return Gulp.src([
            config.bowerDir + '/bootstrap/dist/js/bootstrap.min.js',
            config.bowerDir + '/jquery/dist/jquery.min.js'
        ]).pipe(Gulp.dest(config.jsDest + '/vendor'));
});

Gulp.task('sass', ['bower'], function () {
    Gulp.src(config.sassSrc)
        .pipe(Sass({
            includePaths: [
                config.bowerDir + "/bootstrap/scss",
                config.bowerDir + "/fontawesome/scss"
            ]
        }))
        .pipe(Sass().on('error', Sass.logError))
        .pipe(Rename('site.css'))
        .pipe(Gulp.dest(config.sassDest));
});

Gulp.task('sass:watch', function () {
    Gulp.watch(config.sassSrc, ['sass']);
});

Gulp.task('js:watch', function () {
    Gulp.watch(config.jsSrc, ['javascriptSource']);
});