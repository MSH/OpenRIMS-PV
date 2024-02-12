﻿import Dexie from 'dexie';
import { FormAttachmentValueModel } from '../models/form/form-attachment-value.model';
import { FormValueModel } from '../models/form/form-value.model';
import { SynchStatusModel } from '../models/synchstatus.model';

export class AppDatabase extends Dexie {
    
    forms: Dexie.Table<Form, number>;
    formValues: Dexie.Table<FormValueModel, number>;
    formAttachments: Dexie.Table<FormAttachmentValueModel, number>;
    synchStatus: Dexie.Table<SynchStatusModel, number>;

    constructor() 
    {
        super( "PViMSDatabase" );
      
        var db = this;

        //
        // Define tables and indexes
        //      
        this.version(1).stores({
            forms: '++id, formIdentifier, patientIdentifier, patientName, completeStatus, synchStatus, formType',
            formValues: '++id, formUniqueIdentifier',
            synchStatus: '++id'
        });
        this.version(2).stores({
            formValues: '++id, formid',
        });
        this.version(3).stores({
          formAttachments: '++id, formUniqueIdentifier, formid',
        });

        db.forms.mapToClass(Form);        
    }
}

export class Form {
    id: number;
    created: string;
    formIdentifier: string;
    patientIdentifier: string;
    patientName: string;
    completeStatus: string;
    synchStatus: string;
    formType: string;
    attachment: any;
    hasAttachment: boolean;
    attachment_2: any;
    hasSecondAttachment: boolean;

    formValues: FormValueModel[];
    formAttachments: FormAttachmentValueModel[];
    
    constructor(
        created: string,
        formIdentifier: string, 
        patientIdentifier: string, 
        patientName: string,
        completeStatus: string,
        formType: string,
        attachment: any,
        hasAttachment: boolean,
        attachment_2: any,
        hasSecondAttachment: boolean,
        id?:number) 
    {
        this.created = created;
        this.formIdentifier = formIdentifier;
        this.patientIdentifier = patientIdentifier;
        this.patientName = patientName;
        this.completeStatus = completeStatus;
        this.formType = formType;
        this.synchStatus = 'Not Synched';
        this.attachment = attachment;
        this.hasAttachment = hasAttachment;
        this.attachment_2 = attachment_2;
        this.hasSecondAttachment = hasSecondAttachment;
        if (id) this.id = id;

        // Define navigation properties.
        // Making them non-enumerable will prevent them from being handled by indexedDB
        // when doing put() or add().
        Object.defineProperties(this, {
            formValues: {value: [], enumerable: false, writable: true },
            formAttachments: {value: [], enumerable: false, writable: true }
        });        
    }

    async loadNavigationProperties() {
        [this.formValues] = await Promise.all([
            db.formValues.where('formid').equals(this.id).toArray()
        ]);
        [this.formAttachments] = await Promise.all([
          db.formAttachments.where('formid').equals(this.id).toArray()
      ]);        
    }    
    
    save() {
        return db.transaction('rw', db.forms, db.formValues, db.formAttachments, async() => {
            
            // Add or update our selves. If add, record this.id.
            this.id = await db.forms.put(this);

            // Save all navigation properties (arrays of formValues)
            // Some may be new and some may be updates of existing objects.
            // put() will handle both cases.
            // (record the result keys from the put() operations into Ids
            //  so that we can find local deletes)
            let [valueIds] = await Promise.all ([
                Promise.all(this.formValues.map(value => db.formValues.put(value)))
            ]);
                            
            // Was any value deleted from out navigation properties?
            // Delete any item in DB that reference us, but is not present
            // in our navigation properties:
            await Promise.all([
                db.formValues.where('formid').equals(this.id) // references us
                    .and(value => valueIds.indexOf(value.id) === -1) // Not anymore in our array
                    .delete(),
            ]);

            let [attachmentIds] = await Promise.all ([
              Promise.all(this.formAttachments.map(attachment => db.formAttachments.put(attachment)))
            ]);
                          
            // Was any value deleted from out navigation properties?
            // Delete any item in DB that reference us, but is not present
            // in our navigation properties:
            await Promise.all([
                db.formAttachments.where('formid').equals(this.id) // references us
                    .and(attachment => attachmentIds.indexOf(attachment.id) === -1) // Not anymore in our array
                    .delete(),
            ]);
        });
    }
  }
  export var db = new AppDatabase();