import { TruncatePipe } from './truncate.pipe';

describe('TruncatePipe', () => {
  it('create an instance', () => {
    const pipe = new TruncatePipe();
    expect(pipe).toBeTruthy();
  });
  it('should truncate a string to 10 characters', () => {
    const pipe = new TruncatePipe();
    // pass params
    const ret: any = pipe.transform('abcdefghijklmnopqrstuvwxyzABCDEF', 10);
    expect(ret.length).toBe(10);
  });
  it('should truncate a string to 10 characters plus an ellipsis', () => {
    const pipe = new TruncatePipe();
    // pass params
    const ret: any = pipe.transform(
      'abcdefghijklmnopqrstuvwxyzABCDEF',
      10,
      '...'
    );
    expect(ret).toEqual('abcdefghij...');
  });
});
