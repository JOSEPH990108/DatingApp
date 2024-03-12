import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
export class TextInputComponent implements ControlValueAccessor {

  @Input() label: string = '';
  @Input() type: string = 'text';

  constructor(@Self() public ngControl: NgControl ) {  // the functions are written by the ControlValueAccessor .. so we don't declare any method code // @self ensure local injection
    this.ngControl.valueAccessor = this;               // this allows access to our control when we register/use it inside our controlValueAccessor
  }

  writeValue(obj: any): void {

  }

  registerOnChange(fn: any): void {

  }

  registerOnTouched(fn: any): void {

  }

  get control(): FormControl{
    return this.ngControl.control as FormControl;
  }

}
