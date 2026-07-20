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

export type MatchPredictionRevealItemDto = {
  id: string;
  userId: string;
  user: UserPreviewDto;
  predictedHomeScore: number;
  predictedAwayScore: number;
  predictedAdvancingTeamId: string | null;
  pointsEarned: number | null;
};

export type MatchPredictionsRevealResponseDto = {
  matchId: string;
  areRevealed: boolean;
  predictionCount: number;
  predictions: MatchPredictionRevealItemDto[];
};
