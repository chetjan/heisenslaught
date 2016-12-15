import { DraftPage } from './app.po';

describe('draft App', function() {
  let page: DraftPage;

  beforeEach(() => {
    page = new DraftPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
