import { Component, OnInit } from '@angular/core';
import { DraftAdminService } from './services/draft-admin.service';


@Component({
  selector: 'app-draft-admin',
  templateUrl: './draft-admin.component.html',
  styleUrls: ['./draft-admin.component.scss']
})
export class DraftAdminComponent implements OnInit {

  private draftStats: any;

  constructor(private draftService: DraftAdminService) { }

  async ngOnInit() {
    this.draftStats = await this.draftService.getDraftStats();
  }

}
