function log(constructor: Function) {
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

@log
class Person {
  constructor() {}
}
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
  .then((res: any) => console.log(res));
