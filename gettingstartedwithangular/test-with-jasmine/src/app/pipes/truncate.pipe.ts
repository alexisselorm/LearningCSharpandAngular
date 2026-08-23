import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  standalone: false,
  name: 'truncate',
})
export class TruncatePipe implements PipeTransform {
  transform(value: string, limit: number, ellipsis: string = ''): any {
    return value.length > limit ? value.substring(0, limit) + ellipsis : value;
  }
}
