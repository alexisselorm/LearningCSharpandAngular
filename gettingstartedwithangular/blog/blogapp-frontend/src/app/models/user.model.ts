export class User {
  private id = '';
  private username = '';
  private password = '';

  constructor(username: string, password: string) {
    this.username = username;
    this.password = password;
  }

  getUsername(): string {
    return this.username;
  }
  getPassword(): string {
    return this.password;
  }
  setId(id: string) {
    this.id = id;
  }

  getId(): string {
    return this.id;
  }
}
