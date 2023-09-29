import { Component } from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-base-form',
  template: ` <p>base-form works!</p> `,
  styles: [],
})
export abstract class BaseFormComponent {
  //the form model
  form!: FormGroup;

  getErrors(
    control: AbstractControl,
    displayName: string,
    customMessages: { [key: string]: string } | null = null
  ): string[] {
    var errors: string[] = [];
    Object.keys(control.errors || {}).forEach((key) => {
      switch (key) {
        case 'required':
          errors.push(
            `${displayName} ${customMessages?.[key] ?? 'is required'} `
          );
          break;
        case 'pattern':
          errors.push(
            `${displayName} ${
              customMessages?.[key] ?? 'contains invalid characters'
            } `
          );
          break;
        case 'isDupeField':
          errors.push(
            `${displayName} ${
              customMessages?.[key] ?? 'already exists. Please choose another'
            } `
          );
          break;
        default:
          errors.push(`${displayName} is invalid`);
          break;
      }
    });
    return errors;
  }

  constructor() {}
}
