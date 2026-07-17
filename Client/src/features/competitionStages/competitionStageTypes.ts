export type CompetitionStageType = 1 | 2 | 3 | 4;
export type CompetitionStageStatus = 1 | 2 | 3 | 4;

export type CompetitionStagePreviewDto = {
  id: string;
  competitionSeasonId: string;
  name: string;
  type: CompetitionStageType;
  displayOrder: number;
  status: CompetitionStageStatus;
  isActive: boolean;
};

export type CompetitionStageResponseDto = {
  id: string;
  competitionSeasonId: string;
  name: string;
  type: CompetitionStageType;
  displayOrder: number;
  startDate: string;
  endDate: string;
  status: CompetitionStageStatus;
  isActive: boolean;
};

export type CreateCompetitionStageRequest = {
  competitionSeasonId: string;
  name: string;
  type: CompetitionStageType;
  displayOrder: number;
  startDate: string;
  endDate: string;
  status: CompetitionStageStatus;
  isActive?: boolean;
};

export type CreatedCompetitionStageResponseDto = {
  id: string;
  name: string;
};

export const COMPETITION_STAGE_TYPE = {
  LeagueTable: 1,
  GroupStage: 2,
  Knockout: 3,
  Final: 4,
} as const;

export const COMPETITION_STAGE_STATUS = {
  Upcoming: 1,
  Active: 2,
  Finished: 3,
  Cancelled: 4,
} as const;

export const COMPETITION_STAGE_TYPE_LABEL: Record<CompetitionStageType, string> = {
  1: "Puan durumu",
  2: "Grup aşaması",
  3: "Eleme",
  4: "Final",
};
