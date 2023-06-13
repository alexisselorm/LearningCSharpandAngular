"use strict";
function displayName(emp) {
    return `${emp.lastName}`;
}
console.log(displayName({ firstName: "Alexis", lastName: "Smith", birthDate: "2015-" }));
class Person {
    constructor(firstName, lastName, birthDate) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.birthDate = birthDate;
    }
    get fullName() {
        return `${this.firstName} ${this.lastName}`;
    }
}
const alexis = new Person("Alexis", "Gbeckor-Kove", new Date("12-12-23"));
console.log(alexis.fullName);
