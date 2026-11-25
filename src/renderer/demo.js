window.DEMO = {
  version: '0.1.0',
  account: { displayName: 'Demo Player', username: 'demo_player', level: 27 },
  server: { online: 418, peak: 497, average: 211 },
  game: {
    installed: true,
    build: '20260323.14',
    patch: '2026.11.03-1',
    folder: 'C:\\Games\\RecFlare\\RecRoom',
    sizeGb: 6.4,
    updateAvailable: false,
  },
  rooms: [
    { name: 'RecCenter', by: 'Coach', visits: '48.2K', cheers: '1.1K', hue: 24 },
    { name: 'PaintballArena', by: 'paintballpat', visits: '22.8K', cheers: '640', hue: 350 },
    { name: 'OrbitObby', by: 'orbitmaker', visits: '9.4K', cheers: '212', hue: 262 },
    { name: 'BucketBistro', by: 'bucketfan', visits: '3.1K', cheers: '88', hue: 190 },
  ],
  studio: {
    unity: { ready: true, version: '2022.3.62f1', path: 'C:\\Program Files\\Unity\\Hub\\Editor\\2022.3.62f1' },
    version: 179,
    projects: [
      { id: 'p1', name: 'Rooftop Lounge', edited: '3 hours ago', path: 'D:\\RecFlareStudio\\RooftopLounge' },
      { id: 'p2', name: 'Neon Visor', edited: 'yesterday', path: 'D:\\RecFlareStudio\\NeonVisor' },
      { id: 'p3', name: 'Obby Kit', edited: '5 days ago', path: 'D:\\RecFlareStudio\\ObbyKit' },
    ],
  },
  myRooms: {
    featured: [
      { RoomId: 1, RoomName: 'RecCenter', ImageName: '' },
      { RoomId: 2, RoomName: 'PaintballArena', ImageName: '' },
      { RoomId: 3, RoomName: 'OrbitObby', ImageName: '' },
    ],
    favorites: [
      { RoomId: 2, Name: 'PaintballArena', ImageName: '', Stats: { VisitCount: 22800, CheerCount: 640 } },
      { RoomId: 4, Name: 'BucketBistro', ImageName: '', Stats: { VisitCount: 3100, CheerCount: 88 } },
    ],
    cheered: [
      { RoomId: 3, Name: 'OrbitObby', ImageName: '', Stats: { VisitCount: 9400, CheerCount: 212 } },
    ],
  },
  settings: { closeOnPlay: true, startInVr: false, autoUpdate: true },
};
