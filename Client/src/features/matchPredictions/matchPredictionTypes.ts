export type UserPreviewDto = {
  id: string;
  username: string;
  profileImageUrl: string | null;
  roleName: string;
};

export type MatchPredictionResponseDto = {
  id: string;
  userId: string;
  user: UserPreviewDto;
  matchId: string;
  predictedHomeScore: number;
  predictedAwayScore: number;
  predictedAdvancingTeamId: string | null;
  pointsEarned: number | null;
};
