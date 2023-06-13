"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
function log(constructor) {
    console.log("Constructor log");
}
let firstMethod = function () {
    let promise = new Promise(function (resolve, reject) {
        setTimeout(function () {
            console.log("calling the first method completed");
            resolve({ data: "123" });
        }, 2000);
    });
    return promise;
};
let secondMethod = function (someStuff) {
    let promise = new Promise(function (resolve, reject) {
        setTimeout(function () {
            console.log("calling the second method completed");
            resolve({ newData: someStuff.data + " some more data" });
        }, 2000);
    });
    return promise;
};
let Person = class Person {
    constructor() { }
};
Person = __decorate([
    log
], Person);
let thirdMethod = function (someStuff) {
    let promise = new Promise(function (resolve, reject) {
        setTimeout(function () {
            console.log("calling the third method completed");
            resolve({ result: someStuff.newData + " and even more data" });
        }, 3000);
    });
    return promise;
};
firstMethod()
    .then(secondMethod)
    .then(thirdMethod)
    .then((res) => console.log(res));
