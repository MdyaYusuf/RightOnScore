export type CompetitionGroupPreviewDto = {
  id: string;
  competitionStageId: string;
  name: string;
  displayOrder: number;
  isActive: boolean;
};

export type CreateCompetitionGroupRequest = {
  competitionStageId: string;
  name: string;
  displayOrder: number;
  isActive?: boolean;
};

export type CreatedCompetitionGroupResponseDto = {
  id: string;
  name: string;
};
