import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";

import * as ExcelJS from 'exceljs/dist/exceljs.min.js';
import * as FileSaver from 'file-saver';
import { TranslateService } from '@ngx-translate/core';
import { DatePipe } from '@angular/common';

const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
const EXCEL_EXTENSION = '.xlsx';

declare const ExcelJS: any;

@Injectable({ providedIn: 'root' })
export class ExcelGenService {

  workbook: ExcelJS.Workbook;
  worksheet: any;

  constructor(
      protected httpClient: HttpClient, 
      protected translateService: TranslateService,
      protected datePipe: DatePipe) { }

  public generateExcelForGrid(title: string, headers: string[], mappedResults: any[]) {
    let self = this;

    this.workbook = new ExcelJS.Workbook();

    const itemsForTranslation: string[] = headers.slice();
    itemsForTranslation.push(title);
    itemsForTranslation.push('Date of report');

    self.translateService.get(itemsForTranslation).subscribe((res: string) => {
      this.worksheet = this.workbook.addWorksheet(res[title]);

      let titleRow = this.worksheet.addRow([res[title]]);
      // Set font, size and style in title row.
      titleRow.font = { name: 'Calibri', family: 4, size: 12, bold: true };

      // Add row with current date          
      let subTitleRow = this.worksheet.addRow([res['Date of report'] + ' : ' + this.datePipe.transform(new Date(), 'medium')]);
      
      // Blank Row
      this.worksheet.addRow([]);
  
      // Add Header Row
      let headerOutput: string[] = [];
      headers.forEach(element => {
        headerOutput.push(res[element]);
      })

      let headerRow = this.worksheet.addRow(headerOutput);
      headerRow.font = { bold: true };
  
      // Cell Style : Fill and Border
      headerRow.eachCell((cell, number) => {
        cell.border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }
      });
  
      mappedResults.forEach( (result) => {
        let dataRow = this.worksheet.addRow(Object.values(result));
        dataRow.eachCell((cell, number) => {
          cell.border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }
        });
      });
  
      this.workbook.xlsx.writeBuffer().then((data) => {
        let blob = new Blob([data], { type: EXCEL_TYPE });
        let fileName = `Extract_${this.datePipe.transform(new Date(),"yyyyMMddHHmm")}${EXCEL_EXTENSION}`
        FileSaver.saveAs(blob, fileName);
      });
    });
  }
}
