import { Component, Self, Input } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.css']
})
export class DatePickerComponent implements ControlValueAccessor {

  @Input() label: string = '';
  @Input() type: string = 'text';
  @Input() maxDate: Date | undefined;
  bsConfig: Partial<BsDatepickerConfig> | undefined;

  constructor(@Self() public ngControl: NgControl ) {  // the functions are written by the ControlValueAccessor .. so we don't declare any method code // @self ensure local injection
    this.ngControl.valueAccessor = this; // this allows access to our control when we register/use it inside our controlValueAccessor
    this.bsConfig = {
      containerClass: 'theme-blue',
      dateInputFormat: 'DD MM YYYY'
    }
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
