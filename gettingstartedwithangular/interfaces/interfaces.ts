interface employee {
  firstName: string;
  lastName: string;
  birthDate: string;
}

function displayName(emp: employee) {
  return `${emp.lastName}`;
}

console.log(
  displayName({ firstName: "Alexis", lastName: "Smith", birthDate: "2015-" })
);

class Person {
  private firstName: string;
  private lastName: string;
  private birthDate: Date;

  constructor(firstName: string, lastName: string, birthDate: Date) {
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
