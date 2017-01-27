/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { DraftAdminService } from './draft-admin.service';

describe('DraftAdminService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DraftAdminService]
    });
  });

  it('should ...', inject([DraftAdminService], (service: DraftAdminService) => {
    expect(service).toBeTruthy();
  }));
});
