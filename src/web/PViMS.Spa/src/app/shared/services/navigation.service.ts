import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";
import { AccountService } from "./account.service";
import { Router } from "@angular/router";
import { _routes } from "app/config/routes";
import { RoutePartsService } from "./route-parts.service";
import { MetaPageDetailModel } from "../models/meta/meta-page.detail.model";
import { MetaPageService } from "./meta-page.service";
import { MetaReportDetailModel } from "../models/meta/meta-report.detail.model";
import { MetaReportService } from "./meta-report.service";

interface IMenuItem {
  type: string; // Possible values: link/dropDown/icon/separator/extLink
  name?: string; // Used as display text for item and title for separator type
  state?: string; // Router state
  parameter?: string; // Router state parameter
  icon?: string; // Material icon name
  tooltip?: string; // Tooltip text
  disabled?: boolean; // If true, item will not be appeared in sidenav.
  sub?: IChildItem[]; // Dropdown items
  badges?: IBadge[];
}

interface IChildItem {
  type?: string;
  name: string; // Display text
  state?: string; // Router state
  parameter?: string; // Router state parameter
  icon?: string;
  sub?: IChildItem[];
}

interface IBadge {
  color: string; // primary/accent/warn/hex color codes(#fff000)
  value: string; // Display text
}

@Injectable({ providedIn: 'root' })
export class NavigationService {
  constructor(
    protected _router: Router,
    protected accountService: AccountService,
    protected metaPageService: MetaPageService,
    protected metaReportService: MetaReportService,
    protected routePartsService: RoutePartsService
  ) { }

  currentPortal: string;

  clinicalMenu: IMenuItem[] = [];

  analyticalMenu: IMenuItem[] = [
    {
      type: "separator",
      name: "Main Actions"
    },
    {
      name: "Spontaneous",
      type: "dropDown",
      tooltip: "Spontaneous Actions",
      icon: "dashboard",
      state: "analytical",
      sub: [
        { name: "Reports", state: "reportsearch", parameter: "4096D0A3-45F7-4702-BDA1-76AEDE41B986" },
        { name: "Analyser", state: "spontaneousanalyser" }
      ]
    },
    {
      name: "Active",
      type: "dropDown",
      tooltip: "Active Actions",
      icon: "dashboard",
      state: "analytical",
      sub: [
        { name: "Reports", state: "reportsearch", parameter: "892F3305-7819-4F18-8A87-11CBA3AEE219" },
        { name: "Analyser", state: "activeanalyser" }
      ]
    }
  ];

  reportMenu: IMenuItem[] = [];

  infoMenu: IMenuItem[] = [];

  adminMenu: IMenuItem[] = [
    {
      type: "separator",
      name: "Main Actions"
    },
    {
      name: "Audit Trail",
      type: "link",
      tooltip: "",
      icon: "event_note",
      state: "administration/auditlog"
    },
    {
      name: "Reference Data",
      type: "dropDown",
      tooltip: "",
      icon: "extension",
      state: "administration/reference",
      sub: [
        { name: "Condition Groups", state: "condition" },
        { name: "MedDRA Terms", state: "meddra" },
        { name: "Tests and Procedures", state: "labtest" },
        { name: "Test Results", state: "labresult" }
      ]
    },
    {
      name: "Medicines",
      type: "dropDown",
      tooltip: "",
      icon: "local_hospital",
      state: "administration/reference",
      sub: [
        { name: "Active Ingredients", state: "concept" },
        { name: "Products", state: "medicine" },
      ]
    },
    {
      name: "System Configuration",
      type: "dropDown",
      tooltip: "",
      icon: "settings",
      state: "administration/system",
      sub: [
        { name: "Configuration", state: "config" },
        { name: "Contact Details", state: "contactdetail" },
        { name: "Facilities", state: "facility" },
        { name: "Public Holidays", state: "holiday" },
        { name: "Report Meta Data", state: "reportmeta" }
      ]
    },
    {
      name: "User Configuration",
      type: "dropDown",
      tooltip: "",
      icon: "account_circle",
      state: "administration/user",
      sub: [
        { name: "Users", state: "user" },
        { name: "Roles", state: "role" }
      ]
    },
    {
      name: "Work Configuration",
      type: "dropDown",
      tooltip: "",
      icon: "folder",
      state: "administration/work",
      sub: [
        { name: "Attributes", state: "attributes" },
        { name: "Care Events", state: "careevent" },
        { name: "Datasets", state: "dataset" },
        { name: "Dataset Elements", state: "datasetelement" },
        { name: "Encounter Types", state: "encountertype" },
        { name: "Work Plans", state: "workplan" }
      ]
    }
  ];

  // Icon menu TITLE at the very top of navigation.
  // This title will appear if any icon type item is present in menu.
  iconTypeMenuTitle: string = "Portals";
  // sets iconMenu as default;
  menuItems = new BehaviorSubject<IMenuItem[]>(this.clinicalMenu);
  // navigation component has subscribed to this Observable
  menuItems$ = this.menuItems.asObservable();

  // Supply different menu for different user type.
  publishNavigationChange(menuType: string) 
  {
    switch (menuType) {
      case "clinical-menu":
        this.prepareClinicalMenus();
        this.menuItems.next(this.clinicalMenu);
        break;
        
      case "analytical-menu":
        this.menuItems.next(this.analyticalMenu);
        break;

      case "reports-menu":
        this.prepareReportMenus();
        break;

      case "information-menu":
        this.prepareInfoMenus();
        break;
  
      case "administration-menu":
        this.menuItems.next(this.adminMenu);
        break;
  
      default:
        this.menuItems.next(this.clinicalMenu);
        break;
    }
  }

  // called when route changes through subscription
  setCurrentPortal(portal: string) {
    this.currentPortal = portal; 
  }

  isPortalCurrent(portal:string) {
    return portal == this.currentPortal;
  }

  determineRouteToLanding(): void {
    if(!this.accountService.hasRole('Clinician') && !this.accountService.hasRole('RegClerk')) {
      if(!this.accountService.hasRole('Analyst')) {
        if(!this.accountService.hasRole('Reporter')) {
          if(!this.accountService.hasRole('Publisher')) {
            if(!this.accountService.hasRole('Admin')) {
              if(!this.accountService.hasRole('DataCap')) {
              // user has no roles, do nothing
              }
              else {
                this.routeToFormsLanding();
              }
            }
            else {
              this.routeToAdminLanding();
            }
          }
          else {
            this.routeToPublisherHome();
          }
        }
        else {
          this.routeToPatientTreatmentReport();
        }
      }
      else {
        this.routeToAnalyticalLanding();
      }
    }
    else {
      // route to patient search
      this.routeToClinicalLanding();
    }
  }

  routeToClinicalLanding() : void {
    this._router.navigate([_routes.clinical.patients.search]);
  }

  routeToFormsLanding() : void {
    this._router.navigate([_routes.clinical.forms.landing]);
  }

  routeToAnalyticalLanding() : void {
    this._router.navigate([_routes.analytical.landing]);
  }

  routeToPatientTreatmentReport() : void {
    this._router.navigate([_routes.reports.patienttreatment]);
  }

  routeToPublisherHome() : void {
    this._router.navigate([_routes.information.view(1)]);
  }

  routeToAdminLanding() : void {
    this._router.navigate([_routes.administration.landing]);
  }

  private prepareClinicalMenus(): void {
    this.clinicalMenu = [];

    let newMenu: IMenuItem = {
      type: "separator",
      name: "Main Actions"
    };      
    this.clinicalMenu.push(newMenu);

    // Manually construct clinical menu based on permissions
    if(this.accountService.hasRole('RegClerk') || this.accountService.hasRole('Clinician'))
    {
      let newMenu: IMenuItem = {
        name: "Patients",
        type: "link",
        tooltip: "Search for Patient",
        icon: "people",
        state: "clinical/patientsearch"
      };      
      this.clinicalMenu.push(newMenu);
    }

    if(this.accountService.hasRole('DataCap'))
    {
      let newMenu: IMenuItem = {
        name: "Forms",
        type: "link",
        tooltip: "View Forms for Capture",
        icon: "content_copy",
        state: "clinical/form-landing"
      };      
      this.clinicalMenu.push(newMenu);

      this.menuItems.next(this.clinicalMenu);
    }

    if(this.accountService.hasRole('Clinician'))
    {
      let newMenu: IMenuItem = {
        name: "Encounters",
        type: "link",
        tooltip: "Search for encounter",
        icon: "description",
        state: "clinical/encountersearch"
      };      
      this.clinicalMenu.push(newMenu);

      newMenu = {
        name: "Cohorts",
        type: "link",
        tooltip: "View cohorts",
        icon: "add_to_photos",
        state: "clinical/cohortsearch"
      };      
      this.clinicalMenu.push(newMenu);

      newMenu = {
        name: "PV Feedback",
        type: "link",
        tooltip: "View specialist feedback",
        icon: "info_outline",
        state: "clinical/feedbacksearch"
      };      
      this.clinicalMenu.push(newMenu);
    }

    if(this.accountService.hasRole('RegClerk'))
    {
      let newMenu: IMenuItem = {
        name: "Appointments",
        type: "link",
        tooltip: "View Appointments",
        icon: "date_range",
        state: "clinical/appointmentsearch"
      };      
      this.clinicalMenu.push(newMenu);
    }
  }

  private prepareInfoMenus(): void {
    this.infoMenu = [];
    this.menuItems.next([]);

    console.log('prepare info');
    
    let metaPageList: MetaPageDetailModel[] = [];

    let newMenu: IMenuItem = {
      type: "separator",
      name: "Main Actions"
    };      
    this.infoMenu.push(newMenu);

    this.metaPageService.getAllMetaPages()
      .subscribe(result => {
        metaPageList = result;

        let menuItems: IMenuItem[] = [];

        metaPageList.filter(mp => mp.visible == 'Yes').forEach(function (page) {
          let newMenu: IMenuItem = {
            name: page.pageName,
            type: "link",
            tooltip: page.pageName,
            icon: "content_copy",
            state: "information/pageviewer",
            parameter: page.id.toString()
          };
          menuItems.push(newMenu);
        });
        this.infoMenu = this.infoMenu.concat(menuItems);

        if(this.accountService.hasRole('PublisherAdmin'))
        {
          let newMenu: IMenuItem = {
            name: 'List pages',
            type: "link",
            tooltip: 'List all pages',
            icon: "content_copy",
            state: "information/pagelist"
          };      
          this.infoMenu.push(newMenu);
        }

        this.menuItems.next(this.infoMenu);
      }, error => {
          console.log(error + ' ' + error.statusText);
      });
  }

  private prepareReportMenus(): void {
    this.reportMenu = [];
    this.menuItems.next([]);
    
    let metaReportList: MetaReportDetailModel[] = [];

    let newMenu: IMenuItem = {
      type: "separator",
      name: "Main Actions"
    };      
    this.reportMenu.push(newMenu);

    this.metaReportService.getAllMetaReports()
        .subscribe(result => {
          metaReportList = result;
          let childMenu: IChildItem[] = [];

          let newStandardReportMenu: IMenuItem = {
            type: "dropDown",
            tooltip: "Standard Reports",
            icon: "data_usage",
            state: "reports",
            name: "Standard Reports",
            sub: [
              { name: "Patients on Treatment", state: "system/patienttreatment" },
              { name: "Adverse Events", state: "system/adverseevent" },
              { name: "Adverse Events Frequency", state: "system/adverseeventfrequency" },
              { name: "Causality", state: "system/causality" },
              { name: "Outstanding Visits", state: "system/outstandingvisit" }
            ]
          };      
          this.reportMenu.push(newStandardReportMenu);

          metaReportList.filter(mr => mr.system == 'No' && mr.reportStatus == 'Published').forEach(function (report) {
            let newChildMenu: IChildItem = {
              name: report.reportName,
              state: `reportviewer/${report.id}`,
            };      
            childMenu.push(newChildMenu);
          });

          let newCustomReportMenu: IMenuItem = {
            type: "dropDown",
            tooltip: "Custom Reports",
            icon: "data_usage",
            state: "reports",
            name: "Custom Reports",
            sub: childMenu
          };      
          this.reportMenu.push(newCustomReportMenu);

          if(this.accountService.hasRole('ReporterAdmin'))
          {
            let newMenu: IMenuItem = {
              name: 'List reports',
              type: "link",
              tooltip: 'List all custom reports',
              icon: "content_copy",
              state: "reports/reportlist"
            };      
            this.reportMenu.push(newMenu);
          }
          this.menuItems.next(this.reportMenu);
        }, error => {
            console.log(error + ' ' + error.statusText);
        });
  }
}